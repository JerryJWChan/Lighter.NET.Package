/***檔案上傳處理函式庫***/
//檔案描述資訊記錄物件
function FileInfo() {
    this.ref_id = ''; //檔案的參照id(歸屬記錄的id),若是空值表示首次新增，若有值表示顯示或編輯
    this.f_id = '';      //檔案的唯一識別編號(例如：GUID)若是空值表示首次新增，若有值表示顯示或編輯
    this.ver = 1; //檔案版本號(數字流水號，預設:1)
    this.kind = ''; //檔案分類
    this.temp_id = ''; //檔案流水號(前端用的暫時性編號)
    this.fname_orig = ''; //檔名(原始檔名)
    this.fname = ''; //檔名(新)(for資安：轉成不易識讀的檔名，例如GUID)
    this.ext = ''; //檔案類型(副檔名)
    this.size = 0; //檔案大小(byte)
    this.url = ''; //網址
    this.title = ''; //檔案標題(例如：上傳一張證照檔，可註明「證照名稱」)
    this.metadata = ''; //(頟外的)檔案描述資料([{key1:value1},{key2,value2},...])，格式：JSON
    this.ord = 0; //檔案排序
}

//格式化檔案大小(byte轉成KB,MB,GB表示式)
FileInfo.prototype.formatedSize = function (decimal_digits) {
    /*
     * decimal_digits: 小數點位數
     */
    let kb = 1024;
    let mb = 1024 * 1024;
    let gb = 1024 * 1024 * 1024;
    if (!decimal_digits) decimal_digits = 0;
    if (this.size >= gb) {
        return `${(this.size / gb).toFixed(decimal_digits)} GB`;
    } else if (this.size >= mb) {
        return `${(this.size / mb).toFixed(decimal_digits)} MB`;
    } else {
        return `${(this.size / kb).toFixed(decimal_digits)} KB`;
    }
}

//檔案命令物件(例如：移除、編輯、開啟、預覽、下載、停用、啟用…)
function ModelCommand() {
    this.name = '';  //命令名稱
    this.content = ''; //命令顯示內容
    this.data_key = ''; //命令要處理的data-model的key值
    this.css_class = ''; //css class
    this.handler; //命令處理函式
}

//檔案上傳暫存器
function FileController() {
    this.file_input;     //對應的input type=file元素
    this.browse_button;  //選取檔案按鈕
    this.file_view;      //檔案項目檢視容器
    this.file_store = new DataTransfer(); //檔案暫存器
    this.file_infos = new Map(); //檔案描述資料對照集
    this.min_count = 0;  //最小檔案數，若設0表示選填欄位
    this.max_count = 0;  //最大檔案數，若設0表示無上限
    this.accept = '';    //接受的檔案類型(副檔名，若多組以逗號分隔，例如：.jpg,.png,.jpeg)
    this.size_limit_single = 2 * 1024 * 1024; //單一檔案大小限制(預設2MB)
    this.size_limit_total = 10 * 1024 * 1024; //總檔案大小限制(預設10MB)
    this.size_decimal_digits = 1;   //檔案大小顯示小數點位數    
    this.onAddFile;           //加入檔案事件處理函式
    this.onMaxLimit;      //已達最大檔案數事件處理函式
    this.onMessage;       //訊息提示事件
    this.onReset;         //重置事件
    this.file_commands = []; //檔案命令按鈕(例如：移除、編輯、開啟、預覽、下載)，格式：[{name:'cmd name', content:'cmd button display content',css_class:'css class',handler:function(file_info, file_command){}},...]
    this.original_model = null; //原始model (reset時會用到)
}

//data model or data model list
Object.defineProperty(FileController.prototype, 'model', {
    set(model) {
        /*
         * model: 後端傳回的UploadFileInfoSimpleModel 或 UploadFileInfoBase型別的單獨物件，或List, 或Array
         */

        //先保存在original_model中, reset時會用到
        this.original_model = model;

        if (!model) {
            console.log('FileController.setModel(): model is null');
            return;
        }
        if (typeof model === 'string') {
            console.log('FileController.setModel(): model is not an object');
            return;
        }

        if (typeof model === 'object') {
            if (model.length == 0) return;
            let file_info;
            if (model instanceof Array) {
                //list of model
                for (const m of model) {
                    this.addFileInfo(m);
                }
            } else {
                //single model
                this.addFileInfo(model);
            }
        }
    }
});

//初始化綁定:檔案上傳暫存器
FileController.prototype.setBinding = function (
    {model=null, file_input_id, browse_button_id, file_view_id, min_count, max_count, size_limit_single, size_limit_total, accept = '', size_decimal_digits } = {}
) {
    //清除狀態值
    //(1)清除：file_store
    this.file_store.clearData();
    //(2)清除：file_infos
    this.file_infos.clear();
    //(3)清除:file_commands
    this.file_commands = [];

    //給入初始model
    this.model = model;

    //綁定input type=file元素
    let fileInput = this.getElement(file_input_id, 'input');
    if (fileInput) {
        this.file_input = fileInput;
    }
    else {
        console.log(`FileController.setBinding() failed. the given file_input_id=${file_input_id} not found for a valid input type=file element.`);
    }

    //綁定檔案項目檢視容器
    if (browse_button_id) {
        let browseButton = this.getElement(browse_button_id, 'button');
        if (browseButton) {
            this.browse_button = browseButton;
        } else {
            console.log(`Warning:FileController.setBinding(), the browse_button_id argument of value [${browse_button_id}] is not found for a valid button element id.`);
        }
    }

    //綁定選擇檔案按鈕
    if (file_view_id) {
        let fileView = this.getElement(file_view_id);
        if (fileView) {
            this.file_view = fileView;
        } else {
            console.log(`Warning:FileController.setBinding(), the file_view_id argument of value [${file_view_id}] is not found for a valid html element id.`);
        }
    }

    //檢核min_count
    if (min_count && isNaN(min_count) == false) {
        this.min_count = min_count;
    } else {
        this.min_count = 0;
    }

    //檢核max_count
    if (max_count && isNaN(max_count) == false) {
        this.max_count = max_count;
    } else {
        this.max_count = 0;
    }
    if (this.max_count > 0 && this.max_count < this.min_count) {
        console.log(`FileController.setBinding() failed. when max_count argument > 0 then max_count must larger than min_count`);
    }

    //檢核size_limit_single
    if (size_limit_single && isNaN(size_limit_single) == false) {
        this.size_limit_single = size_limit_single;
    } else {
        this.size_limit_single = 2 * 1024 * 1024; //預設值:2MB
    }
    console.log(`設定單一檔案大小限制=${this.size_limit_single} byte`);

    //檢核size_limit_total
    if (size_limit_total && isNaN(size_limit_total) == false) {
        this.size_limit_total = size_limit_total;
    } else {
        this.size_limit_total = 10 * 1024 * 1024; //預設值:10MB
    }
    console.log(`設定總檔案大小限制=${this.size_limit_total} byte`);

    //檢核size_decimal_digits
    if (size_decimal_digits && isNaN(size_decimal_digits) == false && size_decimal_digits >= 0) {
        this.size_decimal_digits = size_decimal_digits;
    } else {
        this.size_decimal_digits = 1;
    }

    //接受檔案類型, 若綁定的file input元素中有定義accept屬性，則併入
    if (accept) this.accept = accept;
    let fileInputAccept = this.file_input.getAttribute('accept');
    if (fileInputAccept) {
        if (!this.accept) {
            this.accept = fileInputAccept;
        } else {
            //將不重複的檔案類型併入
            let acceptArr = fileInputAccept.split(',');
            for (let i = 0; i < acceptArr.length; i++) {
                if (this.accept.indexOf(acceptArr[i]) < 0) {
                    this.accept += `,${acceptArr[i]}`;
                }
            }
        }
    }

    return this;
}

//設定事件處理函式
FileController.prototype.setEventHandler = function ({ onAddFile, onMessage, onMaxLimit, onReset } = {}) {

    //檢核onAddFile
    if (!onAddFile) {
        console.log(`FileController.setBinding() failed. The onAddFile(addedFile, file_info) event handler is not given.`);
    } else {
        this.onAddFile = onAddFile;
    }

    //input type=file的change事件
    if (this.file_input) {
        //(1)先清除file input元素的檔案
        this.file_input.value = '';

        //(2)再設定change事件
        this.file_input.addEventListener('change', (e) => {
            this.addFile(e);
        });
    }

    if (this.browse_button) {
        //選擇檔案按鈕click事件
        this.browse_button.addEventListener('click', () => {
            //TEST 
            console.log('click event triggered.');
            if (this.file_input) {
                this.file_input.click();
            }
        });
    }

    //onMessage
    if (onMessage) this.onMessage = onMessage;
    //onMaxLimit
    if (onMaxLimit) this.onMaxLimit = onMaxLimit;
    //onReset
    if (onReset) this.onReset = onReset;

    //若前一步驗setBinding()有給入model，則file_infos中有項目，則逐項觸發addFile事件callback
    if (this.onAddFile && this.file_infos.size > 0) {
        for (const info of this.file_infos.values()) {
            //傳遞參數：(1)null表示無加入的檔案 (2)此次檔案的file_info
            this.onAddFile(null, info);
        }

        //若(加入後)已達檔案數上限
        if (this.can_add_more == false) {
            //將選取按鈕disable
            if (this.browse_button) {
                this.browse_button.disabled = true;
            }
            //觸發事件
            if (this.onMaxLimit) {
                this.onMaxLimit(this.file_count);
            }
        }
    }

    return this;
}

//加入檔案描述資訊(回傳一個file_info物件)
FileController.prototype.addFileInfo = function (
    {
        ref_id = '',
        f_id = '',
        ver = 1,
        kind = '',
        fname_orig = '',
        fname='',
        ext = '',
        size = 0,
        url = '',
        title = '',
        metadata = '',
        ord = 0
    } = {},
    display_view = true  //是否將加入的file_info顯示在file_view容器中
) {
    if (!fname_orig) {
        console.log('addFileInfo() failed. fname_orig argument missing');
        return undefined;
    }
    if (!ref_id) ref_id = '';
    if (!f_id) f_id = '';
    if (!ver) ver = 1;
    if (!kind) kind = '';
    if (!size) size = 0;


    if (!ord) ord = 0;
    let info = new FileInfo();
    info.ref_id = ref_id;
    info.f_id = f_id;
    info.ver = ver;
    info.kind = kind;
    info.fname_orig = fname_orig;
    info.fname = fname;
    info.ext = ext;
    info.size = size;
    info.url = url;
    info.title = title;
    info.temp_id = this.createTempId();
    info.metadata = metadata;
    info.ord = ord;
    let key = info.temp_id;
    this.file_infos.set(key, info); //加入file_info，以temp_id為key

    return info;
}

//加入檔案命令按鈕(例如：移除、編輯、開啟、預覽、下載)
FileController.prototype.addFileCommand = function ({
    name,
    content,
    css_class,
    handler
} = {}) {
    let cmd = new ModelCommand();
    cmd.name = name;
    cmd.content = content;
    cmd.css_class = css_class;
    cmd.handler = handler;
    this.file_commands.push(cmd);
    return this;
}

//是否可再加入檔案
Object.defineProperty(FileController.prototype, 'can_add_more', {
    get() {
        if (!this.max_count || this.max_count == 0) return true; //未設定上限
        return this.file_infos.size < this.max_count;
    }
});

//是否達到最小檔案數量
Object.defineProperty(FileController.prototype, 'is_meet_min_count', {
    get() {
        if (!this.min_count || this.min_count == 0) return true; //未設定下限
        return this.file_infos.size >= this.min_count;
    }
});

//檔案數
Object.defineProperty(FileController.prototype, 'file_count', {
    get() {
        return this.file_infos.size;
    }
});

//全部檔案大小(byte)
Object.defineProperty(FileController.prototype, 'total_size', {
    get() {
        if (this.file_infos.size == 0) return 0;
        let sum = [...this.file_infos.values()].map(x => x.size).reduce((s, n) => s + n);
        return sum;
    }
});

//取得指定型別的元素
FileController.prototype.getElement = function (element_id, element_type_name) {
    /*
     * element_id:元素id
     * element_type_name:元素型別名稱
     */

    if (!element_id || element_id == '') return undefined;

    let element;
    if (typeof element_id === 'string') {
        element_id = element_id.startsWith('#') ? element_id : `#${element_id}`;
        element = document.querySelector(element_id);
    } else {
        return undefined;
    }

    if (!element) return undefined;

    if (element_type_name) {
        let node_name = element_type_name.toLowerCase();
        if (element.nodeName.toLowerCase() != node_name) {
            console.log(`the [${element_id}] is not a ${node_name} element type.`);
            return undefined;
        }
    }

    return element;
}

//產生暫時性Id(格式：[亂數首碼]-[亂數中碼]-[亂數尾碼]，其中首碼和尾碼各有10的6次方種可能性)
FileController.prototype.createTempId = function () {
    return `${Math.floor(Math.random() * 1000000)}-${Math.floor(Math.random() * 1000000)}-${Math.floor(Math.random() * 1000000)}`;
}

//加入選取檔案
FileController.prototype.addFile = function (e) {
    e.preventDefault();
    e.stopPropagation();

    if (!this.file_input.files || this.file_input.files.length == 0) return;

    //若(加入前)已達檔案數上限
    if (this.can_add_more == false) {
        //將選取按鈕disable
        if (this.browse_button) {
            this.browse_button.disabled = true;
        }
        //觸發事件
        if (this.onMaxLimit) {
            this.onMaxLimit(this.file_count);
        }
        return;
    }

    //逐一加入檔案
    //let exist = false;
    for (let file of this.file_input.files) {
        let msg = '';
        //檢核1:若相同檔名已存在，則重新命名+訊息提醒
        //exist = false;
        for (let i = 0; i < this.file_store.items.length; i++) {
            if (this.file_store.items[i].getAsFile().name == file.name) {
                let newFilename = this.createNewFilename(file.name);
                console.log(`[${file.name}]檔名已存在。自動重新命名成[${newFilename}]`);
                file = new File(file, newFilename, { type: file.type });
                break;
            }
        }
        //if (exist) continue;

        //檢核2:單一檔案大小是否超過限制
        if (file.size > this.size_limit_single) {
            if (this.onMessage) {
                msg = `[${file.name}]大小超過單一檔案上限${this.formatSize(this.size_limit_single, this.size_decimal_digits)},已略過。`;
                msg += `The size of [${file.name}] exceeds the maximum limit for single file ${this.formatSize(this.size_limit_single, this.size_decimal_digits)}. Therefore, the selected file is omitted. Choose another file or reduce the filesize.`;
                this.onMessage(msg);
            }
            continue;
        }

        //檢核3:總檔案大小是否超過限制
        let totalSize = this.total_size + file.size; 
        if (totalSize > this.size_limit_total) {
            if (this.onMessage) {
                msg = `[${file.name}]使總檔案大小超過上限${this.formatSize(this.size_limit_total, this.size_decimal_digits)},已略過。`;
                msg += `The selected file [${file.name}] leads to the total filesize exceeds the maximum limit ${this.formatSize(this.size_limit_single, this.size_decimal_digits)}. Therefore, the selected file is omitted. Choose another file or reduce the filesize.`;
                this.onMessage(msg);
            }
            continue;
        }

        //檢核4:檔案類型
        if (this.accept) {
            let ext = file.name.split('.').pop().toLowerCase();
            let acceptArr = this.accept.split(',').map(x => x.trim().replace(/^\./, ''));
            if (acceptArr.indexOf(ext) < 0) {
                if (this.onMessage) {
                    msg = `[${file.name}]不是可接受的檔案類型(${this.accept}),已略過。`;
                    msg += `The selectd file [${file.name}] is not an acceptable file format including (${this.accept}), Therefore, omitted. Choose another file.`;
                    this.onMessage(msg);
                }
                continue;
            }
        }

        //加入暫存器
        this.file_store.items.add(file);
        //加入描述資料對照集
        let ext = '';
        let parts = file.name.split('.');
        if (parts.length > 1) ext = parts.pop();
        let info = this.addFileInfo({
            fname_orig: file.name,
            ftype: ext,
            size: file.size,
            ver: 1
        });

        //呼叫onAddFile()處理函式
        if (this.onAddFile) {
            //傳遞參數：(1)此次加入的檔案 (2)此次檔案的file_info
            this.onAddFile(file, info);
        }
        //判斷是否已達檔案數上限
        if (this.can_add_more == false) {
            break;
        }
    }

    //若(加入後)已達檔案數上限
    if (this.can_add_more == false) {
        //將選取按鈕disable
        if (this.browse_button) {
            this.browse_button.disabled = true;
        }
        //觸發事件
        if (this.onMaxLimit) {
            this.onMaxLimit(this.file_count);
        }
    }

    //清除file input元素的檔案
    this.file_input.value = '';

    //file_input.value = '';
    console.log('client file stored count=' + this.file_store.files.length);
}

//移除選取檔案(傳入file_info或temp_id)
FileController.prototype.removeFile = function (file_info_or_temp_id) {
    let info, temp_id;
    if (file_info_or_temp_id instanceof FileInfo) {
        info = file_info_or_temp_id;
        temp_id = info.temp_id;
    } else {
        temp_id = file_info_or_temp_id;
        info = this.file_infos.get(temp_id);
    }

    if (info) {
        try {
            //移除暫存檔(只包含新加入的檔案，其.fname無值,.fname_orig有值)
            if (!info.fname) {
                for (let i = 0; i < this.file_store.items.length; i++) {
                    if (this.file_store.items[i].getAsFile().name == info.fname_orig) {
                        this.file_store.items.remove(i);
                        break;
                    }
                }
            }

            //移除file_info
            let key = info.temp_id;
            this.file_infos.delete(key);
            //將選取按鈕enable
            if (this.browse_button && this.can_add_more) {
                this.browse_button.disabled = false;
            }

        } catch (err) {
            console.log(`warning: remove temp_id=[${temp_id}] from the temp file storage of file input of name [${this.file_input.name}] failed.`);
            console.log(err.message);
        }
    } else {
        console.log(`warning: the temp_id=[${temp_id}] not found in the file_info collection of the temp file storage of file input of name [${this.file_input.name}].`);
    }

}

//重置(model)
FileController.prototype.reset = function () {
    //(1)清除file_store
    this.file_store.clearData();
    //(2)清除file_infos
    this.file_infos.clear();
    //(3)重設成初始model
    this.model = this.original_model;
    //(4)browse_button啟用設定
    if (this.browse_button) {
        this.browse_button.disabled = !this.can_add_more;
    }

    let fileInfos = [...this.file_infos.values()];
    if (this.onReset) {
        this.onReset(fileInfos);
    }
    return fileInfos;
}

//產生一個命令按鈕
FileController.prototype.createCommandButton = function (command) {
    /*
     * comamnd: ModelCommand物件
     */

    let btn = document.createElement('button');
    btn.setAttribute('type', 'button');

    //if (!(command instanceof ModelCommand)) {
    //    console.log(`createCommandButton() failed. [command] must be a ModelCommand object.`);
    //    return undefined;
    //}

    btn.setAttribute('type', 'button');
    btn.setAttribute('data-key', command.data_key);
    btn.innerHTML = command.content;
    btn.classList = command.css_class;
    if (!command.handler) {
        console.log(`設定錯誤：未指定command.name=${command.name},command.content=${command.content}的handler事件處理函式`);
    } else {
        btn.addEventListener('click', (e) => {
            command.handler(e, command.data_key);
        });
    }
    return btn;
}

//從template複製一份model檢視(div元素，內含欄位樣版)
FileController.prototype.cloneViewFromTemplate = function (template, model_key) {
    let view;
    if (!template) {
        console.log(`cloneViewFromTemplate() failed. the [template] is null`);
        return document.createElement('div');
    }
    let div = document.createElement('div');
    div.innerHTML = template.innerHTML;
    view = div.firstElementChild;
    view.setAttribute('data-key', model_key);
    return view;
}

//將file_info套進view tempalte
FileController.prototype.applyModelToView = function ({ model, view, converters, commands } = {}) {
    /*
     * converters陣列格式：
     * [
     *  {
     *      prop_name:'property name',
     *      html: false / true,
     *      convert:(x)=>{return y;}
     *  }
     * ],
     * commands陣列格式：
     * [
     *  {
     *      name:'command name',
     *      content:'command button content',
     *      css_class: 'css class',
     *      handler:function(e,dataKey,model){}
     *  }
     * ]
     */

    let elmList;
    let converter;
    let value;
    let isHtml;
    let nodeName;
    if (!converters) converters = [];
    for (const prop in model) {
        elmList = view.querySelectorAll(`[data-field="${prop}"]`);
        if (elmList && elmList.length > 0) {
            for (const elm of elmList) {
                converter = converters.find(x => x.prop_name == prop);
                if (!converter) {
                    isHtml = false;
                    value = model[prop];
                } else {
                    value = converter.convert(model[prop]);
                    isHtml == converter.html;
                }

                nodeName = elm.nodeName.toLowerCase();
                if (nodeName == 'select') {
                    //datatype: string
                    element.value = value;
                }
                else if (nodeName == 'input') {
                    let nodeType = element.getAttribute('type').toLowerCase();
                    if (nodeType == 'checkbox') {
                        //datatype: bool (true/false)
                        element.checked = ConvertToBoolean(value);
                    }
                    else {
                        //datatype: string
                        element.value = value;
                    }
                }
                else if (nodeName == 'textarea') {
                    element.value = value;
                }
                else if (nodeName == 'span' || nodeName == 'div') {
                    if (!isHtml) {
                        elm.textContent = value;
                    } else {
                        elm.innerHTML = value;
                    }
                } else if (nodeName == 'a') {
                    elm.href = value;
                } else if (nodeName == 'img') {
                    elm.src = value;
                }
                else {
                    console.log(`[${prop}] value not set due no matching input type or tagName`);
                }
            }
        }
    }

    //commands(按鈕)的內容是html
    let cmdContainer = view.querySelector('[data-commands]');
    if (cmdContainer) {
        if (commands && commands.length > 0) {
            commands.forEach(cmd => {
                //將產生的命令按鈕加入此cmdContainer容器中
                let btn = this.createCommandButton(cmd, model );
                cmdContainer.appendChild(btn)
            });
        }
    }
}

//建立新檔名(為重複的檔案名稱尾部加上遞增序號)
FileController.prototype.createNewFilename = function (filename_orig) {
    let index = filename_orig.lastIndexOf('.');
    let fname = '', ext = '', newFilename = '', serialNo = 0;
    if (index >= 0) {
        ext = filename_orig.substring(index + 1);
        fname = filename_orig.substring(0, index);
    } else {
        fname = filename_orig;
    }
    let pattern = /\((\d+)\)$/;
    let matches = fname.match(pattern);
    if (matches && matches.length > 1) {
        serialNo = parseInt(matches[1]);
        fname = fname.replace(/\(\d+\)$/, '');
    }
    serialNo++;
    if (ext) {
        newFilename = `${fname}(${serialNo}).${ext}`;
    } else {
        newFilename = `${fname}(${serialNo})`;
    }

    return newFilename;
}

//格式化檔案大小(byte轉成KB,MB,GB表示式)
FileController.prototype.formatSize = function (fileLengthInByte, decimal_digits) {
    /*
     * fileLengthInByte: 檔案大小(byte)
     * decimal_digits: 小數點位數
     */
    let kb = 1024;
    let mb = 1024 * 1024;
    let gb = 1024 * 1024 * 1024;
    if (!decimal_digits) decimal_digits = 0;
    if (fileLengthInByte >= gb) {
        return `${(fileLengthInByte / gb).toFixed(decimal_digits)} GB`;
    } else if (fileLengthInByte >= mb) {
        return `${(fileLengthInByte / mb).toFixed(decimal_digits)} MB`;
    } else {
        return `${(fileLengthInByte / kb).toFixed(decimal_digits)} KB`;
    }
}
