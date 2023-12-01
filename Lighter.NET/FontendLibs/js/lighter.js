//<Mobile_Detection>
/*
 * 此「偵測是否行動裝置」的區段中的變數和函式名稱前綴都加上__(雙底線)
 * 是為了避免當引用的頁面中有另外引用到其他常用js框架時，而與其他js框架的偵測行動裝置的函式相衝突的可能性
 */
var __isMobile = false;

//(注意：當PC或NB搭配touch screen時，視同行動裝置)
function __DetectMobile() {
    try { document.createEvent("TouchEvent"); return true; }
    catch (e) { return false; }
}

__isMobile = __DetectMobile();

//</Mobile_Detection>

/*function alias(簡化元素選取語法)*/
//選取單一元素(符合指定的css selector)，傳回DOM物件
function _(selector) {
    let found = document.querySelector(selector);
    if (!found) {
        console.log(`[${selector}] not found. check if the selector is valid or missing the starting # symbol for id lookup.`);
    }
    return found;
}


//取得所有符合selector的元素
function _All(selector) {
    return $GetElementArray(selector);
}

//取得指定型別的元素
function $GetElement(element_or_id, element_type_name) {
    /*
     * element_or_id:元素或元素id
     * element_type_name:元素型別名稱
     */

    if (!element_or_id || element_or_id == '') return undefined;

    let element;
    if (typeof element_or_id === 'string') {
        let element_id = element_or_id.startsWith('#') ? element_or_id : `#${element_or_id}`;
        //(1)find by id
        element = _(element_id);
        //(2)find by name (ex. for input type=radio)
        if (!element) {
            let element_name = element_id.substring(1);
            element = _(`input[name="${element_name}"]`);
        }
    } else {
        element = element_or_id;
    }

    if (element_type_name) {
        let node_name = element_type_name.toLowerCase();
        if (!element || element.nodeName.toLowerCase() != node_name) {
            console.log(`the [${element_or_id}] is not a ${node_name} element type.`);
            return undefined;
        }
    }

    return element;
}

//取得物件陣列
function $GetElementArray(element_or_selectors) {
    /*
     * element_or_selectors: 單一元素物件，或逗號分隔的css selector，或元素nodeList集合
     */

    if (!element_or_selectors) return [];
    if (typeof element_or_selectors === 'string') {
        let selectorArr = SplitToSelectors(element_or_selectors);
        let objArr = [];
        selectorArr.forEach(x => {
            let found = document.querySelectorAll(x);
            if (found && found.length > 0) {
                objArr.push(...found);
            }
        });
        return objArr;

    } else if (typeof element_or_selectors === 'object' && Array.isArray(element_or_selectors)) {
        return element_or_selectors;
            
    } else if (element_or_selectors instanceof NodeList) {
        if (element_or_selectors.length > 0) {
            return [...element_or_selectors];
        } else {
            return [];
        }
    } else if (element_or_selectors instanceof Element || element_or_selectors instanceof HTMLDocument) {
        return [element_or_selectors];
    } else if (element_or_selectors instanceof HTMLCollection) {
        if (element_or_selectors.length > 0) {
            return [...element_or_selectors];
        } else {
            return [];
        }
    } else {
        return [];
    }

}

/**Utility functions**/
function $IsEmptyObject(obj) {
    return (obj &&
            Object.keys(obj).length === 0 &&
            obj.constructor === Object);
}

/**Auto Complete**/
//顯示auto complete
function $ShowAutoComplete(target, select) {
    let pos = $CalculateAutoCompletePosition(target);
    select.style.setProperty('top', `${pos.top}px`, 'important');
    select.style.setProperty('left', `${pos.left}px`, 'important');
    select.style.setProperty('width', `${pos.width}px`, 'important');
    let matchCount = select.getAttribute('data-match-count') ?? '0';
    let adjustHeight = Math.min(parseInt(matchCount), 5) * pos.height;
    select.style.setProperty('height', `${adjustHeight}px`, 'important');

    let isShown = !select.classList.contains('hide');
    if (isShown) {
        console.log('already shown');
        return;
    } else {
        select.setAttribute('data-auto-completed', '0');
        select.selectedIndex = -1;
        select.classList.remove('hide');
    }

}

//隱藏auto complete
function $HideAutoComplete(select) {
    select.classList.add('hide');
}

//計算auto complete顯示位置
function $CalculateAutoCompletePosition(target) {
    const rect = target.getBoundingClientRect();
    let top = rect.bottom + window.scrollY;
    let left = rect.left + window.scrollX;
    let width = rect.right - rect.left;
    let height = rect.bottom - rect.top;
    return { top: top, left: left, width: width, height: height };
}

//keyup事件處理
function $HandleSelectKeyup(e, target, select, oncompleteCallback) {
    if (e.isComposing || e.keyCode === 229) {
        return;
    }

    if (e.code == 'Enter' || e.code == 'Space') {
        $SetAutoCompleteValue(target, select, oncompleteCallback);
        $HideAutoComplete(select);
    }

    if (e.code == 'Tab') {
        if (select.selectedIndex >= 0) {
            $SetAutoCompleteValue(target, select, oncompleteCallback);
        }
        $HideAutoComplete(select);
    }

    if (e.code == 'Escape') {
        $HideAutoComplete(select);
        target.focus();
    }
}

//設定自動完成所選定的值
function $SetAutoCompleteValue(target, select, oncompleteCallback) {
    if (select.selectedIndex < 0) {
        console.log(`$SetAutoCompleteValue() aborted. No selected option`);
        return;
    }
    let isCompleted = select.getAttribute('data-auto-completed');
    if (isCompleted == '1') return;

    select.setAttribute('data-auto-completed', '1');

    let value = select.options[select.selectedIndex].value;
    let text = select.options[select.selectedIndex].text;
    target.setAttribute('data-auto-complete-value', value);
    target.value = text;
    if (oncompleteCallback) {
        oncompleteCallback({ value, text, target, select });
    }
    target.focus();
}

//mouse down事件處理
function $HandleSelectMousedown(target) {
    target.setAttribute('data-goto-autocomplete', '1');
}

//mouse up事件處理
function $HandleSelectMouseup(e, target, select, oncompleteCallback) {
    $SetAutoCompleteValue(target, select, oncompleteCallback);
}

//select blue事件處理
function $HandleSelectBlur(target, select, oncompleteCallback) {
    $SetAutoCompleteValue(target, select, oncompleteCallback);
    $HideAutoComplete(select)
}

//過濾auto complete清單
function $FilterAutoCompleteList(target, select, mustMatch, samePairArr) {
    //檢核
    if (samePairArr && !Array.isArray(samePairArr)) {
        console.log(`warning: $FilterAutoCompleteList() samePairArr is not an valid array`);
        samePairArr = [];
    } 

    let value = target.value;
    let matchCount = 0;
    let itemCount = select.options.length;
    if (itemCount == 0) return;
    let firstIndex = -1;
    if (value) {
        let found = false;
        let keywords = [value];
        if (samePairArr && samePairArr.length > 0) {
            for (let i = 0; i < samePairArr.length; i++) {
                if (Array.isArray(samePairArr[i]) && samePairArr[i].length == 2) {
                    if (samePairArr[i][0] && samePairArr[i][1]) {
                        keywords.push(value.replace(samePairArr[i][0], samePairArr[i][1]));
                    } else {
                        console.log(`warning: $FilterAutoCompleteList() samePairArr[${i}] is not an valid array of 2 non-empty element`);
                    }
                    
                } else {
                    console.log(`warning: $FilterAutoCompleteList() samePairArr[${i}] is not an valid array of 2 element`);
                }
            }
        }

        for (let i = 0; i < itemCount; i++) {
            found = false;
            for (let j = 0; j < keywords.length; j++) {
                if (select.options[i].text.indexOf(keywords[j]) >= 0) {
                    found = true;
                    break;
                }
            }

            if (found) {
                if (firstIndex == -1) firstIndex = i;
                matchCount++;
                select.options[i].classList.remove('hide');
            } else {
                select.options[i].classList.add('hide');
            }
        }
    } else {
        for (let i = 0; i < itemCount; i++) {
            select.options[i].classList.remove('hide');
        }
        firstIndex = 0;
        matchCount = itemCount;
    }

    select.setAttribute('data-match-count', matchCount);
    select.setAttribute('data-first-index', firstIndex);
    if (matchCount > 0) {
        $ShowAutoComplete(target, select);
        ClearValidationHint(null, target);
    } else {
        $HideAutoComplete(select);
        if (mustMatch) {
            SetValidationHint(null, '(無符合項目/No matched item)', target);
        }
    }
}

//focus事件處理
function $HandleTextboxFocus(e) {
    let txt = e.currentTarget;
    txt.setAttribute('data-goto-autocomplete', '0');
}

//blus事件處理
function $HandleTextboxBlur(e,target, select, mustMatch) {
    let isAutoComplete = target.getAttribute('data-goto-autocomplete');
    if (isAutoComplete == '1') {
        return;
    } else {
        window.setTimeout(function () {
            let isAutoComplete = target.getAttribute('data-goto-autocomplete');
            if (isAutoComplete == '1') {
                return;
            }
            let value = target.value;
            if (value && mustMatch === true) {
                let match = $SelectContainsOptionText(select, value); //NOTE:此處比較的是選項文字
                if (!match) {
                    e.preventDefault();
                    e.stopPropagation();
                    let fieldLabel = target.getAttribute('data-field-label');
                    if (!fieldLabel) fieldLabel = target.id;
                    if (!fieldLabel) fieldLabel = target.getAttribute('name');
                    if (!fieldLabel) fieldLabel = target.getAttribute('data-field');
                    let message = `<b class="bold-600">[<b class="text-red">${fieldLabel}</b>]欄位不接受提示項目以外的內容</b>，<b class="text-blue">請重新輸入關鍵字後，選擇提示列表中的項目</b>`;
                    let msg = { msgType: 'warning', caption: '系統提示', message: message };
                    $PopMessage(msg);
                    return;
                }
            }
            $HideAutoComplete(select);
        }, 250);

    }
}

//keyup事件處理
function $HandleTextboxKeyUp(e,target,select,mustMatch, samePairArr) {
    if (e.isComposing || e.keyCode === 229) {
        return;
    }

    switch (e.key) {
        case 'Tab':
        case 'Escape':
            $HideAutoComplete(select);
            return;

        case 'ArrowDown':
        case 'ArrowUp':
            if (select.options.length > 0) {
                $ShowAutoComplete(target, select);
                target.setAttribute('data-goto-autocomplete', '1');
                select.focus();
                try {
                    let firstIndex = parseInt(select.getAttribute('data-first-index'));
                    select.selectedIndex = firstIndex;
                    
                } catch {
                    select.selectedIndex = -1;
                }
            }
            return;

        case 'Shift':
        case 'Control':
        case 'Alt':
        case 'Pause':
        case 'CapsLock':
        case 'PageUp':
        case 'PageDown':
        case 'End':
        case 'Home':
        case 'ArrowLeft':
        case 'ArrowRight':
        case 'PrintScreen':
        case 'Insert':
            return;

        default:
            let value = target.value;
            $FilterAutoCompleteList(target, select, mustMatch, samePairArr);
    }

}


/**Lighter Object**/
function _$(target_elemenets_or_selector, { acceptUndefinedTarget } = {}) {
    return new $LighterObject(target_elemenets_or_selector, { acceptUndefinedTarget: acceptUndefinedTarget });
}
function $LighterObject(target_elemenets_or_selector, { acceptUndefinedTarget } = {}) {
    //target
    this.targets = undefined;
    this.acceptUndefinedTarget = acceptUndefinedTarget;
    if (target_elemenets_or_selector) {
        this.targets = $GetElementArray(target_elemenets_or_selector);
        if (!this.targets || this.targets.length == 0 && !acceptUndefinedTarget) {
            console.log(`$LighterObject() failed. the given argument:${target_elemenets_or_selector} is not valid elements or selector.`);
        }
    } else {
        if (!acceptUndefinedTarget) {
            console.log(`$LighterObject() failed. the target_elemenets_or_selector argument is missing.`);
        }
    }
}

Object.defineProperty($LighterObject.prototype, 'valid', {
    get() {
        return this.targets && this.targets.length > 0;
    }
});

Object.defineProperty($LighterObject.prototype, '0', {
    get() {
        if (!this.targets || this.targets.length == 0) {
            console.log('Failed to access the target element [0]. the target of $LighterObject is undefined.');
            return undefined;
        }
        return this.targets[0];
    }
});

Object.defineProperty($LighterObject.prototype, 'length', {
    get() {
        if (!this.targets || this.targets.length == 0) {
            if (!this.acceptUndefinedTarget) {
                console.log('Warning: Accessing length property failed due to the target of $LighterObject is undefined.');
            }
            return 0;
        }
        return this.targets.length;
    }
});

$LighterObject.prototype.info = function ({ withText } = {}) {
    if (!this.targets || this.targets.length == 0) {
        console.log('Warning: Accessing targetInfo property failed due to the target of $LighterObject is undefined.');
        return 'undefined';
    }

    let idAttr = this.targets[0].id ? ` id="${this.targets[0].id}"` : '';
    let nameAttr = this.targets[0].name ? ` id="${this.targets[0].name}"` : '';
    let classAttr = this.targets[0].getAttribute('class');
    classAttr = classAttr ? ` class="${classAttr}"` : '';
    let text = '';
    if (withText) text = this.targets[0].innerText;
    return `target(length=${this.targets.length}):<${this.targets[0].tagName.toLowerCase()}${idAttr}${nameAttr}${classAttr}>${text}`
}

$LighterObject.prototype.equal = function (another) {
    if (!another) {
        console.log(`Warning: equal(), the another argument is undefined`);
        return false;
    }
    if (!(another instanceof $LighterObject)) {
        console.log(`Warning: equal(), the another argument is not of type $LighterObject`);
        return false;
    }
    if (!this.valid) {
        console.log(`Warning: equal(), the current $LighterObject has no target element`);
        return false;
    }
    if (!another.valid) {
        console.log(`Warning: equal(), the another $LighterObject has no target element`);
        return false;
    }
    if (this.length != another.length) {
        console.log(`Warning: equal(), the length of the two $LighterObject is different`);
        return false;
    }

    if (this.length == 1) {
        return this.targets[0] == another.targets[0];
    } else {
        let length = this.length;
        let allEqual = true;
        for (let i = 0; i < length; i++) {
            if (this.targets[i] != another.targets[i]) {
                allEqual = false;
                break;
            }
        }
        return allEqual;
    }

}

$LighterObject.prototype.first = function () {
    if (!this.valid) {
        console.log('first() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    return new $LighterObject(this.targets[0]);
}

$LighterObject.prototype.last = function () {
    if (!this.valid) {
        console.log('last() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    return new $LighterObject(this.targets[this.length-1]);
}

$LighterObject.prototype.item = function (index) {
    if (!this.valid) {
        console.log('item() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    if (index < 0 || index >= this.length) {
        console.log(`Error:item() faild. the index argument is out of range. the maximum index should be less than or equal to ${this.length -1}`);
        return undefined;
    }
    return new $LighterObject(this.targets[index]);
}

$LighterObject.prototype.add = function (targetElement) {
    if (!this.valid) {
        console.log('add() faild. the target of $LighterObject is undefined.');
        return this;
    }

    if (!targetElement) {
        console.log('add() faild. the targetElement argument is undefined.');
        return this;
    }

    if (targetElement instanceof Element) {
        this.targets.push(targetElement);
    } else if (targetElement instanceof NodeList) {
        this.targets.push([...targetElement]);
    } else {
        console.log('add() faild. the targetElement argument is not a valid Element or NodeList type.');
        return this;
    }
    return this;
}

$LighterObject.prototype.remove = function (targetElement) {
    if (!this.valid) {
        console.log('remove() faild. the target of $LighterObject is undefined.');
        return this;
    }

    if (!targetElement) {
        console.log('add() faild. the targetElement argument is undefined.');
        return this;
    }

    if ((!(targetElement instanceof Element)) && (!(targetElement instanceof NodeList))) {
        console.log('remove() faild. the targetElement argument is not a valid Element or NodeList type.');
        return this;
    }
    let filtered = this.targets.filter(x => x != targetElement);
    this.targets = filtered;
    return this;
}

$LighterObject.prototype.forEach = function (callback) {
    if (!this.valid) {
        console.log('forEach() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    if (!callback) {
        console.log('forEach() faild. the callback argument is undefined.');
        return undefined;
    }
    for (let elm of this.targets) {
        let $elm = new $LighterObject(elm);
        callback($elm);
    }
}

$LighterObject.prototype.show = function (displayMode) {
    Show(this.targets, displayMode);
    return this;
}

$LighterObject.prototype.hide = function () {
    Hide(this.targets);
    return this;
}

$LighterObject.prototype.clear = function () {
    Clear(this.targets);
    return this;
}

$LighterObject.prototype.enable = function () {
    if (!this.valid) {
        console.log('enable() faild. the target of $LighterObject is undefined.');
        return ;
    }
    this.targets.forEach(x => { $SetEnable(x); });
}

$LighterObject.prototype.disable = function () {
    if (!this.valid) {
        console.log('disable() faild. the target of $LighterObject is undefined.');
        return;
    }
    this.targets.forEach(x => { $SetDisable(x); });
}

$LighterObject.prototype.value = function (value) {
    if (!this.valid) {
        console.log('value() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    if (arguments.length == 0) {
        return GetValue(this.targets[0]);
    } else {
        SetValue(this.targets[0], value);
        return this;
    }
}

$LighterObject.prototype.text = function (text) {
    if (arguments.length == 0) {
        if (!this.valid) {
            console.log('text() faild. the target of $LighterObject is undefined.');
            return undefined;
        }
        return GetText(this.targets[0]);
    } else {
        if (!this.valid) {
            console.log('text() faild. the target of $LighterObject is undefined.');
            return this;
        }
        SetText(this.targets[0], text);
        return this;
    }
}

$LighterObject.prototype.html = function (html) {
    if (arguments.length == 0) {
        if (!this.valid) {
            console.log('html() faild. the target of $LighterObject is undefined.');
            return undefined;
        }
        return this.targets[0].innerHTML;
    } else {
        if (!this.valid) {
            console.log('html() faild. the target of $LighterObject is undefined.');
            return this;
        }
        this.targets[0].innerHTML = html;
        return this;
    }
}

$LighterObject.prototype.attr = function (attrName, attrValue) {
    if (!this.valid) {
        console.log('attr() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    if (arguments.length == 1) {
        return this.targets[0].getAttribute(attrName);
    } else if (arguments.length == 2) {
        this.targets[0].setAttribute(attrName, attrValue);
        return this;
    } else {
        console.log('attr() faild. Mising attrName and attrValue argument.');
        return undefined;
    }
}

$LighterObject.prototype.getData = function (dataAttributeName, datatype) {
    if (!this.valid) {
        console.log('getData() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let data = GetData(this.targets[0], dataAttributeName);
    if (!datatype) {
        return data;

    } else {
        if (datatype == 'int' || datatype == 'inteter' || datatype == 'number') {
            if (data === undefined || data == '' || isNaN(data)) {
                console.log(`warning: getData(), 0 is returned due to dataAttributeName=${dataAttributeName} is undefined or not a number.`);
                return 0;

            } else {
                return parseInt(data);
            }
        } else {
            //do to : 其他資料型別
            return data;
        }
    }
}

$LighterObject.prototype.setData = function (dataAttributeName, value) {
    if (!this.valid) {
        console.log('setData() faild. the target of $LighterObject is undefined.');
        return this;
    }
    SetData(this.targets[0], dataAttributeName, value);
    return this;
}

$LighterObject.prototype.data = function (dataAttributeName, value) {
    if (!this.valid) {
        console.log('data() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    if (arguments.length == 0) {
        console.log('data() failed. missing argument of dataAttributeName');
        return undefined;
    }

    if (arguments.length == 1) {
        return GetData(this.targets[0], dataAttributeName);
    } else {
        SetData(this.targets[0], dataAttributeName, value);
        return this;
    }
}

$LighterObject.prototype.addClass = function (...cssClass) {
    if (!this.valid) {
        console.log('addClass() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    for (const t of this.targets) {
        t.classList.add(...cssClass);
    }
}

$LighterObject.prototype.removeClass = function (...cssClass) {
    if (!this.valid) {
        console.log('removeClass() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    for (const t of this.targets) {
        t.classList.remove(...cssClass);
    }
}

$LighterObject.prototype.containsClass = function (cssClass) {
    if (!this.valid) {
        console.log('containsClass() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    return this.targets[0].classList.contains(cssClass);
}

$LighterObject.prototype.flag = function (flagName) {
    if (!this.valid) {
        console.log('getFlag() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    return new $ElementFlag(this.targets[0], flagName);
}

$LighterObject.prototype.parent = function (selector) {
    if (!this.valid) {
        console.log('parent() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let element;
    if (!selector) {
        element = this.targets[0].parentElement;
    } else {
        element = this.targets[0].closest(selector);
    }

    if (element) {
        return new $LighterObject(element);
    } else {
        return undefined;
    }
}

$LighterObject.prototype.child = function (selector) {
    if (!this.valid) {
        console.log('child() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let child;
    if (!selector) {
        child = this.targets[0].firstChild;        
    } else {
        child = this.targets[0].querySelector(selector);
    }

    if (child) {
        return new $LighterObject(child);
    } else {
        return undefined;
    }
}

$LighterObject.prototype.childs = function (selector) {
    if (!this.valid) {
        console.log('child() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let childs;
    if (!selector) {
        childs = this.targets[0].childNodes;
    } else {
        childs = this.targets[0].querySelectorAll(selector);
    }

    if (childs && childs.length > 0) {
        return new $LighterObject(childs);
    } else {
        console.log(`Warning: childs(), the selector argument:${selector} does not match any element.`);
        return undefined;
    }
}

$LighterObject.prototype.childElement = function (selector) {
    if (!this.valid) {
        console.log('child() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let child;
    if (!selector) {
        child = this.targets[0].firstElementChild;
    } else {
        child = this.targets[0].querySelector(selector);
    }

    if (child) {
        return new $LighterObject(child);
    } else {
        return undefined;
    }

}

$LighterObject.prototype.childElements = function (selector) {
    if (!this.valid) {
        console.log('child() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let childs;
    if (!selector) {
        childs = this.targets[0].children;
    } else {
        childs = this.targets[0].querySelectorAll(selector);
    }

    if (childs && childs.length > 0) {
        return new $LighterObject(childs);
    } else {
        return undefined;
    }

}

$LighterObject.prototype.sibling = function (selector) {
    if (!this.valid) {
        console.log('sibling() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let parent = this.parent();
    if (parent) {
        let sibling;
        if (selector) {
            sibling = parent.childElement(`:scope > ${selector}`);
            if (sibling) {
                sibling.remove(this.targets[0]); //排除自己本身
            }
        } else {
            sibling =  this.nextElementSibling();
        }

        if (!sibling || !sibling.valid) {
            console.log(`Warning:sibling(), the sibling selector '${selector}' does not match any element.`);
            return undefined;
        }
        return sibling;
    } else {
        console.log('Warning:sibling(), the parent of current element is not found, resulting in no sibling found.');
        return undefined;
    }
}

$LighterObject.prototype.siblings = function (selector) {
    if (!this.valid) {
        console.log('siblings() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let parent = this.parent();
    if (parent) {
        let siblings;
        let matches;
        if (selector) {
            matches = parent.childElements(`:scope > ${selector}`);
        } else {
            matches = parent.childElements();
        }

        if (matches && matches.length > 0) {
            siblings = matches.remove(this.targets[0]); //排除自己本身
        }

        if (siblings && siblings.length > 0) {
            return siblings;
        } else {
            console.log(`Warning:siblings(), the siblings selector '${selector}' does not match any element.`);
            return undefined;
        }
    } else {
        console.log('Warning:sibling(), the parent of current element is not found, resulting in no sibling found.');
        return undefined;
    }
}

$LighterObject.prototype.nextElementSibling = function () {
    if (!this.valid) {
        console.log('nextElementSibling() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let sibling = this.targets[0].nextElementSibling;
    if (sibling) {
        return new $LighterObject(sibling);
    } else {
        return undefined;
    }
}

$LighterObject.prototype.previousElementSibling = function () {
    if (!this.valid) {
        console.log('previousElementSibling() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    let sibling = this.targets[0].previousElementSibling;
    if (sibling) {
        return new $LighterObject(sibling);
    } else {
        return undefined;
    }
}

$LighterObject.prototype.scopeParent = function (scopeName) {
    if (!this.valid) {
        console.log('scopeParent() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let selector = '[data-scope]';
    if (scopeName) selector = `[data-scope="${scopeName}"]`;
    let found = this.targets[0].closest(selector);
    if (found) {
        return new $LighterObject(found);
    } else {
        console.log(`warning: scopeParent() not found for selector=${selector}`);
        return undefined;
    }
}

//取得在相同的data-scope之下的單一符合條件的元素
$LighterObject.prototype.scopeElement = function (selector) {
    if (!this.valid) {
        console.log('scopeElement() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let $scopeParent = this.scopeParent();
    if ($scopeParent && $scopeParent.valid) {
        return $scopeParent.childElement(selector);
    } else {
        return undefined;
    }
}

//取得在相同的data-scope之下的全部符合條件的元素
$LighterObject.prototype.scopeElements = function (selector) {
    if (!this.valid) {
        console.log('scopeElement() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    let $scopeParent = this.scopeParent();

    if ($scopeParent && $scopeParent.valid) {
        return $scopeParent.childElements(selector);
    }
    return undefined;
}

//載入內容(innerHtml)
$LighterObject.prototype.loadContent = async function ({ url, params, method, reqContentType, reqHeaders, onLoad } = {}) {
    if (!this.valid) {
        console.log('loadContent() faild. the target of $LighterObject is undefined.');
        return undefined;
    }
    if (!url) {
        console.log('loadContent() faild. the url argument is null or empty.');
        return undefined;
    }

    if (!method) method = 'POST';
    if (!params) params = {};

    let json = await FetchJson(url, method, params, reqContentType, reqHeaders);
    if (json.success) {
        if (json.data) {
            if (json.data.html) {
                this.targets[0].innerHTML = json.data.html;
            } else {
                this.targets[0].innerHTML = json.data;
            }
        } else {
            console.log(`warning: loadContent(), the json.data of the response is null or empty.`);
        }
        if (onLoad) {
            onLoad();
        }
    } else {
        ShowFetchJsonFailMessage(null, json);
    }

}

//is in viewport
$LighterObject.prototype.isInViewport = function (side) {
    if (!this.valid) {
        console.log(`warning: isInViewport() aborted because the targets is not undefined for current $LighterObject.`);
        return false;
    }
    return $IsInViewport(this.targets[0], side);
}

//attribute binding
$LighterObject.prototype.bind = function (attribute, target, changeCallback) {
    if (!this.valid) {
        console.log(`warning: bind() aborted because the targets is not undefined for current $LighterObject.`);
        return false;
    }

    if (!target) {
        console.log(`warning: bind() aborted because the target argument is not undefined`);
        return false;
    }

    if (target instanceof $LighterObject) {
        if (target.valid) {
            if (this.targets.length == 1) {
                //1 to 1 or 1 to M binding
                return $SetElementBinding(this.targets[0], attribute, target.targets, changeCallback);

            } else {
                if (this.targets.length == target.targets.length) {
                    //M to M binding
                    return $SetElementBinding(this.targets, attribute, target.targets, changeCallback);
                } else {
                    console.log(`warning: bind() aborted because the source element counts is not equal to target element counts when doing M to M binding`);
                    return false;
                }
            }
        } else {
            console.log(`warning: bind() aborted because the target is not a valid $LighterObject nor a HTMLElement`);
            return false;
        }

    } else {
        return $SetElementBinding(this.targets[0], attribute, target, changeCallback);
    }
}

//retrieveauto complete
$LighterObject.prototype.autoComplete = async function ({id, labelText, itemList, url, method, requestArgs, size, cssClass, mustMatch, oncompleteCallback, samePairArr} = {}) {
    /*
     * id: id for auto complete select tag
     * labelText: 欄位顯示名稱
     * itemList: 字串陣列(如果直接給入itemList直不需要再向url request)(用以產生dataList tag)
     * url: 要request回valueList的endpoint url
     * method: request method
     * requestArgs: request參數data object
     * size: auto complete表列要顯示的列數大小(預設值5，超過列數時出現scrollbar)
     * cssClass:自動完成列表的cssClass(預設值：auto-complete-list)
     * mustMatch:必須符合自動完成選項(預設：false)
     * oncompleteCallback:自動完成(選定項目)時要執行的callback, 帶回參數{value,text,target,select}
     * samePairArr: 異體字組陣列
     */

    if (!this.valid) {
        console.log('autoComplete() faild. the target of $LighterObject is undefined.');
        return undefined;
    }

    //若未給itemList但有給url，則向url request回itemList
    if (!itemList && url) {
        let json = await FetchJson(url, method, requestArgs)
        if (json.success) {
            itemList = json.data
        } else {
            let argJson = '';
            try {
                argJson = JSON.stringify(requestArgs);
            } catch {
                argJson = 'javascript 物件語法錯誤';
            }
            console.log(`autoComplete() fetch itemList failed. url=${url}, requestArgs=${argJson}`);
            return undefined;
        }
    }

    if (!itemList) {
        console.log(`autoComplete() failed. itemList is undefined.`);
        return undefined;
    }

    if (!Array.isArray(itemList)) {
        console.log(`autoComplete() failed. itemList is not a valid javascript array.`);
        return undefined;
    }

    if (!cssClass) cssClass = 'auto-complete-list';

    if (!size) size = 5;
    //逐一針對bindingTarget建立select tag和綁定auto complete行為
    for (let t of this.targets) {

        //將target element 的html tag的autocomplete屬性設成off(防止browser顯示干擾畫面)
        let acAttr = t.getAttribute('autocomplete');
        if (!acAttr || acAttr != 'off') {
            t.setAttribute('autocomplete','off');
        }

        t.classList.add('auto-complete-element');
        if (labelText) {
            t.setAttribute('data-field-label',labelText);
        }

        //建立查詢用的select tag
        let sel = document.createElement('select');
        //將select的font-size調整成和目標元素一樣
        let style = GetActualStyle(t);
        let fontSize = style.getPropertyValue('font-size');
        if (fontSize) {
            sel.style.setProperty('font-size', fontSize);
        }

        //check id重複
        if (_(`#${id}`)) {
            console.log(`Warning: autoComplete(), the id argument [value=${id}] is not unique. There has been an element on the plage with the same id. Therefore, the id argument has been modified to [auto_${id}]`);
            id = `auto_${id}`;
        }

        if (id) sel.id = id;
        let itemCount = itemList.length;
        sel.size = size;
        sel.setAttribute('data-match-count', itemCount);
        sel.setAttribute('data-first-index', -1);
        sel.classList.add(cssClass);
        sel.classList.add('hide');

        //建立選項
        for (let i = 0; i < itemCount; i++) {
            let op = document.createElement('option');
            op.value = itemList[i].value;
            op.text = itemList[i].text;
            sel.appendChild(op);
        }

        //事件綁定
        sel.addEventListener('keyup', (e) => {
            $HandleSelectKeyup(e, t, sel, oncompleteCallback);
        });

        sel.addEventListener('mouseup', (e) => {
            $HandleSelectMouseup(e, t, sel, oncompleteCallback);
        });

        sel.addEventListener('mousedown', () => {
            $HandleSelectMousedown(t);
        });

        sel.addEventListener('blur', (e) => {
            $HandleSelectBlur(t, sel, oncompleteCallback);
        });

        //加至target的下一個位置
/*        t.insertAdjacentElement('afterend', noMatchSpan);*/
        t.insertAdjacentElement('afterend', sel);

        //document.body.appendChild(sel);

        t.addEventListener('focus', $HandleTextboxFocus);
        t.addEventListener('blur', (e) => {
            $HandleTextboxBlur(e, t, sel, mustMatch);
        });
        t.addEventListener('keyup', (e) => {
            $HandleTextboxKeyUp(e, t, sel, mustMatch, samePairArr);
        });
    }

}

//add css class(多組以空白分隔)
$LighterObject.prototype.addCssClass = function (classList) {
    if (!this.valid) {
        console.log('addCssClass() faild. the target of $LighterObject is undefined.');
        return ;
    }

    for (const elm of this.targets) {
        elm.classList.add(classList);
    }
}

//remove css class(多組以空白分隔)
$LighterObject.prototype.removeCssClass = function (classList) {
    if (!this.valid) {
        console.log('removeCssClass() faild. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.classList.remove(classList);
    }
}

//設定檢核提示訊息
$LighterObject.prototype.setValidationHint = function (hintText) {
    if (!this.valid) {
        console.log('setValidationHint() faild. the target of $LighterObject is undefined.');
        return;
    }
    let tagName = this.targets[0].tagName.toUpperCase();
    if ('INPUT,SELECT,TEXTAREA'.indexOf(tagName) < 0) {
        console.log(`Warning:setValidationHint(), the target element type [${tagName}] is not supported for this function.`);
        return;
    }

    SetValidationHint(null, hintText, this.targets[0]);
}

//清除檢核提示訊息
$LighterObject.prototype.clearValidationHint = function () {
    if (!this.valid) {
        console.log('clearValidationHint() faild. the target of $LighterObject is undefined.');
        return;
    }

    let tagName = this.targets[0].tagName.toUpperCase();
    if ('INPUT,SELECT,TEXTAREA'.indexOf(tagName) < 0) {
        console.log(`Warning:setValidationHint(), the target element type [${tagName}] is not supported for this function.`);
        return;
    }
    ClearValidationHint(null, this.targets[0]);
}

//click event handler
$LighterObject.prototype.click = function (eventHandler) {
    if (!this.valid) {
        console.log('click() faild. the target of $LighterObject is undefined.');
        return;
    }

    for (let elm of this.targets) {
        elm.addEventListener('click', eventHandler);
    }
}

//change event handler
$LighterObject.prototype.change = function (eventHandler) {
    if (!this.valid) {
        console.log('change() falied. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.addEventListener('change', eventHandler);
    }
}

//focus event handler
$LighterObject.prototype.focus = function (eventHandler) {
    if (!this.valid) {
        console.log('focus() falied. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.addEventListener('focus', eventHandler);
    }
}

//blur event handler
$LighterObject.prototype.blur = function (eventHandler) {
    if (!this.valid) {
        console.log('blur() falied. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.addEventListener('blur', eventHandler);
    }
}


//keyup event handler
$LighterObject.prototype.keyup = function (eventHandler) {
    if (!this.valid) {
        console.log('keyup() falied. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.addEventListener('keyup', eventHandler);
    }
}

//keydown event handler
$LighterObject.prototype.keydown = function (eventHandler) {
    if (!this.valid) {
        console.log('keydown() falied. the target of $LighterObject is undefined.');
        return;
    }

    for (const elm of this.targets) {
        elm.addEventListener('keydown', eventHandler);
    }
}

//mouseover event handler
$LighterObject.prototype.mouseover = function (eventHandler) {
    if (!this.valid) {
        console.log('mouseover() faild. the target of $LighterObject is undefined.');
        return;
    }

    for (let elm of this.targets) {
        elm.addEventListener('mouseover', eventHandler);
    }
}

//mouseout event handler
$LighterObject.prototype.mouseout = function (eventHandler) {
    if (!this.valid) {
        console.log('mouseout() faild. the target of $LighterObject is undefined.');
        return;
    }

    for (let elm of this.targets) {
        elm.addEventListener('mouseout', eventHandler);
    }
}

//產生暫時性Id(格式：[亂數首碼]-[亂數中碼]-[亂數尾碼]，首、中、尾碼各有10的6次方種可能性)
function $CreateTempId() {
    return `${Math.floor(Math.random() * 1000000)}-${Math.floor(Math.random() * 1000000)}-${Math.floor(Math.random() * 1000000)}`;
}

//設定頁面載入DOMContentLoaded事件的執行動作
function $Ready(callback) {
    if (
        document.readyState === "complete" ||
        (document.readyState !== "loading" && !document.documentElement.doScroll)
    ) {
        callback();
    } else {
        document.addEventListener("DOMContentLoaded", callback);
    }
}

//設定Window.onload事件要執行的動作
function $OnLoad(callback) {
    window.addEventListener('load', callback);
}

/*Culture */
let $Culture = '';
$Ready(() => {
    let script = document.querySelector('script[src*="lighter.js"]');
    if (script) {
        let queryStr = new URL(script.src).searchParams;
        if (queryStr) {
            let lang = queryStr.get('Lang');
            if (!lang) lang = queryStr.get('lang');
            $Culture = (lang) ? lang : 'zh-TW';
        }
    }
    if ($Culture == '') $Culture = 'zh-TW';
    console.log(`$Culture=${$Culture}`);
});

/*lighter.js Initialization*/
function $InitializeLighterJs() {
    try {
        //initialize navbars
        $InitializeNavbar();
        //monitor window resize and fire rwdLevelChanged event
        window.addEventListener('resize', () => {
            if ($CurrentPageSize.rwdLevelChanged) {
                let rwdLevel = $CurrentPageSize.rwdLevel;
                let width = $CurrentPageSize.width;
                let height = $CurrentPageSize.height;
                $EventHub().fireEvent('windowRwdChange', { rwdLevel: rwdLevel, width: width, height: height });
            }
        });
        //fire first windowRwdChange event
        let rwdLevel = $CurrentPageSize.rwdLevel;
        let width = $CurrentPageSize.width;
        let height = $CurrentPageSize.height;
        $EventHub().fireEvent('windowRwdChange', { rwdLevel: rwdLevel, width: width, height: height });

        console.log('lighter.js initialized.');
    } catch (e) {
        console.log(`Warning: $InitializeLighterJs() failed. error=${e.message}`);
    }
}

window.addEventListener('load', $InitializeLighterJs);

//關閉視窗(若失敗，則導頁至fallbackUrl)
function $CloseWindow(fallbackUrl) {
    try {
        if (fallbackUrl) {
            setTimeout(() => {
                location.href = fallbackUrl;
            }, 1500);
        }
        window.close();
    } catch {
        if (fallbackUrl) {
            location.href = fallbackUrl;
        } else {
            alert('The browser does not support window.close() function. You may need to close the window by yourself.');
        }
    }
}

function $GetOKText() {
    let text = 'OK';
    if ($Culture == 'zh-TW' || $Culture == 'hant' || $Culture == 'Hant') {
        text = '確定';
    }
    if ($Culture == 'zh-CN' || $Culture == 'hans' || $Culture == 'Hans') {
        text = '确定';
    }
    return text;
}

function $GetCancelText() {
    let text = 'Cancel';
    if ($Culture == 'zh-TW' || $Culture == 'hant' || $Culture == 'Hant') {
        text = '取消';
    }
    if ($Culture == 'zh-CN' || $Culture == 'hans' || $Culture == 'Hans') {
        text = '取消';
    }
    return text;
}
//「確定」按鈕文字
$OKText = $GetOKText();
//「取消」按鈕文字
$CancelText = $GetCancelText();

/*Lighter Dialog Message Box*/
//跳窗顯示訊息
function $PopMessage({ title, message, msgType, caption, text, callback } = {}) {
    /*
     * title:訊息框標頭文字
     * message:訊息內容(可包含Html)
     * msgType:訊息種類 (info:資訊, warning:提示/警告, error:錯誤)
     * caption: title的alias name, 二者傳入任一個都可以
     * text: message的alias name, 二者傳入任一個都可以
     * callback:按確定或關閉鈕後，要回呼的函式
     */

    //msgType=='confirm'時另外處理
    if (msgType == 'confirm') {
        return $PopConfirm({ title, message, msgType, caption, text });
    }

    this.template = `<form method="dialog" class="dialog-msg-title">
            <span class="dialog-msg-title-text">訊息標題</span>
            <button type="submit" value="cancel" class="dialog-btn-close-circle">&times;</button>
        </form>
        <div class="dialog-msg-content-container">
            <section class="dialog-msg-content">

            </section>
            <section class="dialog-action-hint" style="margin-top:1rem;">
                &nbsp;
            </section>
        </div>
        <hr class="dialog-hr" />
        <form method="dialog" class="dialog-msg-footer">
            <button type="submit" value="ok" class="dialog-btn dialog-btn-ok">${$GetOKText()}</button>
        </form> `;

    //default
    if (!title) title = '';
    if (!message) message = '';
    if (!msgType) msgType = 'info';
    if (!caption) caption = '';
    if (!text) text = '';
    //make a msgBox
    let msgBox = document.createElement('dialog');
    let msgSize = "middle";
    if (!message) message = '';
    if (message && message.length < 50) msgSize = 'small';
    if (message && message.length > 250) msgSize = 'big';
    msgBox.innerHTML = this.template;
    msgBox.classList.add('lighter-dialog-msg');
    msgBox.classList.add(`msg-${msgSize}`);
    msgBox.classList.add('msg-screen-center');
    msgBox.setAttribute('aria-role', 'dialog');
    msgBox.setAttribute('data-role', 'message-box');
    document.body.appendChild(msgBox);
    msgBox.querySelector('.dialog-msg-content').innerHTML = message + text;
    let dialogTitle = msgBox.querySelector('form.dialog-msg-title');
    let titleText = dialogTitle.querySelector('.dialog-msg-title-text');
    let okBtn = msgBox.querySelector('button.dialog-btn-ok');
    dialogTitle.classList.add(`dialog-msg-${msgType}`);
    let fullCaption = title + caption;
    if (!fullCaption) fullCaption = '系統提示';
    titleText.innerText = fullCaption;
    //close event handler
    msgBox.addEventListener('close', () => {
        msgBox.remove();
        if (callback) callback();
    });
    //popup
    msgBox.show();
    //set focus
    okBtn.focus();
    return msgBox;
}

//跳窗顯示訊息
function $PopConfirm({ title, message, caption, text } = {}) {
    /*
     * title:訊息框標頭文字
     * message:訊息內容(可包含Html)
     * caption: title的alias name, 二者傳入任一個都可以
     * text: message的alias name, 二者傳入任一個都可以
     * NOTE: 回傳值：true(OK/確定), false(Cancel/取消)
     */

    this.template = `<form method="dialog" class="dialog-msg-title">
            <span class="dialog-msg-title-text">訊息標題</span>
            <button type="submit" value="cancel" class="dialog-btn-close-circle">&times;</button>
        </form>
        <div class="dialog-msg-content-container">
            <section class="dialog-msg-content">

            </section>
            <section class="dialog-action-hint" style="margin-top:1rem;">
                &nbsp;
            </section>
        </div>
        <hr class="dialog-hr" />
        <form method="dialog" class="dialog-msg-footer">
            <button type="submit" value="cancel" class="dialog-btn dialog-btn-cancel">${$GetCancelText()}</button>
            <button type="submit" value="ok" class="dialog-btn dialog-btn-ok">${$GetOKText()}</button>
        </form> `;

    //default
    let msgType = 'confirm';
    if (!title) title = '';
    if (!message) message = 'no message';
    if (!caption) caption = '';
    if (!text) text = '';
    //make a msgBox
    let msgBox = document.createElement('dialog');
    let msgSize = "middle";
    if (!message) message = '';
    if (message && message.length < 50) msgSize = 'small';
    if (message && message.length > 250) msgSize = 'big';
    msgBox.innerHTML = this.template;
    msgBox.classList.add('lighter-dialog-msg');
    msgBox.classList.add(`msg-${msgSize}`);
    msgBox.classList.add('msg-screen-center');
    msgBox.setAttribute('aria-role', 'dialog');
    msgBox.setAttribute('data-role', 'message-box');
    msgBox.querySelector('.dialog-msg-content').innerHTML = message + text;
    let dialogTitle = msgBox.querySelector('form.dialog-msg-title');
    let titleText = dialogTitle.querySelector('.dialog-msg-title-text');
    let btnCancel = msgBox.querySelector('button.dialog-btn-cancel');
    dialogTitle.classList.add(`dialog-msg-${msgType}`);
    titleText.innerText = title + caption;
    //close event handler
    let pms = new Promise((res, rej) => {
        msgBox.addEventListener('close', () => {
            msgBox.remove();
            let rValue = msgBox.returnValue == 'ok';
            //if (!rValue) rValue = 'cancel';
            res(rValue);
        });
    });
    //popup
    document.body.appendChild(msgBox);
    msgBox.showModal();
    //set focus
    btnCancel.focus();
    return pms;
}

//元素是否在Viewport之內
function $IsInViewport(element, side) {
    /*
     * side: which side should be inside viewport, ex. left, top, right, bottom
     */
    const rect = element.getBoundingClientRect();
    const elmWidth = rect.right - rect.left;
    const elmHeight = rect.bottom - rect.top;
    const vpWidth = window.innerWidth || document.documentElement.clientWidth;
    const vpHeight = window.innerHeight || document.documentElement.clientHeight;

    if (elmWidth <= vpWidth && elmHeight <= vpHeight) {
        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= vpHeight &&
            rect.right <= vpWidth
            );
    } else {
        if (side) {
            switch (side) {
                case 'left':
                    return rect.left >= 0 && rect.left <= vpWidth;
                case 'top':
                    return rect.top >= 0 && retc.top <= vpHeight;
                case 'right':
                    return rect.right >= 0 && rect.right <= vpWidth;
                case 'bottom':
                    return rect.bottom >= 0 && rect.bottom <= vpHeight;
            }
        } else {
            return ((rect.left >= 0 && rect.left <= vpWidth && rect.top >= 0 && retc.top <= vpHeight)
                || (rect.right >= 0 && rect.right <= vpWidth && rect.bottom >= 0 && rect.bottom <= vpHeight)
            );
        }
    }

}

//畫面捲動至頂端
function $ScrollTop(behavior) {
    if (!behavior) behavior = 'smooth';
    if (document.documentElement) {
        // For Chrome, Firefox, IE and Opera
        document.documentElement.scrollTo({
            top: 0,
            behavior: behavior
        });
    } else {
        // For Safari
        document.body.scrollTo({
            top: 0,
            behavior: behavior
        });
    }
}

let __btnBackToTopVisible = false;
//回頁首按鈕
let __btnBackToTop = undefined;
//當畫面捲動超過200時，顯示回頁首按鈕
function __backToTopSwitch() {
    if (window.scrollY > 200) {
        if (__btnBackToTopVisible == false) {
            __btnBackToTopVisible = true;
            Show(__btnBackToTop);
        }
    } else {
        if (__btnBackToTopVisible == true) {
            __btnBackToTopVisible = false;
            Hide(__btnBackToTop);
        }
    }
}

//註冊回頁首按鈕
function RegisterBackToTopButton(btnSelector) {
    __btnBackToTop = _(btnSelector);
}


//事件匯集器
let $EventHubTarget = document.createElement('button');
function $EventHub() {
    return new $EventHubObject();
}
function $EventHubObject() {}
//聽取自訂事件
$EventHubHandlerManager = new Map();
$EventHubObject.prototype.listen = function (customEventName, eventHandler, callerName) {
    //if (!callerName) callerName = 'any';
    if (callerName) {
        let key = `${customEventName}_${callerName}`;
        let handler = $EventHubHandlerManager.get(key);
        if (handler) {
            $EventHubTarget.removeEventListener(customEventName, handler);
        }
        $EventHubHandlerManager.set(key, handler);
    }

    handler = (e) => { eventHandler(e.detail); };
    $EventHubTarget.addEventListener(customEventName, handler);

}
//觸發自訂事件
$EventHubObject.prototype.fireEvent = function (customEventName, args) {
    $EventHubTarget.dispatchEvent(new CustomEvent(customEventName, {
        bubbles: false,
        detail:args
    }));
}
//觸發內建click事件
$EventHubObject.prototype.click = function (target_element_or_id) {
    let target = $GetElement(target_element_or_id);
    if (target) {
        let e = new MouseEvent('click', {
            view: window,
            bubbles: true,
            cancelable: true
        });
        target.dispatchEvent(e);
    } else {
        console.log(`$EventHub().click() failed. target[${target_element_or_id}] not found.`);
    }
}
//觸發內建change事件
$EventHubObject.prototype.change = function (target_element_or_id) {
    let target = $GetElement(target_element_or_id);
    if (target) {
        let e = new Event('change', {
            view: window,
            bubbles: true,
            cancelable: true
        });
        target.dispatchEvent(e);
    } else {
        console.log(`$EventHub().change() failed. target[${target_element_or_id}] not found.`);
    }
}

/*Device Info*/
/*全域：RWD尺寸變化點分級輔助器*/
let RWDCutPointHelper = new $RWDCutPointObject();
/*目前畫面尺寸+RWD等級狀態*/
let $CurrentPageSize = new $CurrentPageSizeObject();
//目前畫面尺寸+RWD等級狀態物件定義
function $CurrentPageSizeObject() {
    this._rwdLevel = -1;
}
//width
Object.defineProperty($CurrentPageSizeObject.prototype, 'width', {
    get() {
        return document.documentElement.clientWidth || window.innerWidth;
    }
});
//height
Object.defineProperty($CurrentPageSizeObject.prototype, 'height', {
    get() {
        return document.documentElement.clientHeight || window.innerHeight;
    }
});
//rwdLevel
Object.defineProperty($CurrentPageSizeObject.prototype, 'rwdLevel', {
    get() {
        return RWDCutPointHelper.calculateRwdLevel(this.width);
    },
    set(level) {
        this._rwdLevel = level;
    }
});
//rwdLevelChanged
Object.defineProperty($CurrentPageSizeObject.prototype, 'rwdLevelChanged', {
    get() {
        let currentlevel = this.rwdLevel;
        if (currentlevel != this._rwdLevel) {
            this._rwdLevel = currentlevel;
            return true;
        } else {
            return false;
        }
    }
});


/*Radio button Group Utils*/
$RadioGroups = []; //radio選項群組儲存庫
//radio選項群組物件
function $RadioGroupObject(radioName, isBindingLabel) {
    if (radioName) {
        if (radioName.startsWith('#')) {
            radioName = radioName.substring(1);
        }
    }

    if (isBindingLabel === undefined) {
        isBindingLabel = true; //預設與radio button右側的label綁定(click連動)
    }
    
    this.groupName = radioName; //群組名稱(即元素name)
    this.items = undefined;     //radio元素集合
    this.previousCheckedItem = undefined; //上一個選定的元素
    this.onchange = undefined; //選項變更事件處理函式
    let items = $GetElementArray(`input[type="radio"][name="${radioName}"]`);
    if (!items || items.length == 0) {
        console.log(`$RadioGroupObject() failed. radio with name of ${radioName} is not found.`);
        return undefined;
    } else {
        this.items = items;
        //註冊事件(radio)
        for (const i of this.items) {
            i.addEventListener('change', () => {
                this.notifyChange.apply(this);
            });
        }

        //綁定label連動
        if (isBindingLabel) {
            this.bindLabel();
        }
    }

    this.previousCheckedItem = this.checkedItem;
}

//選定的radio項目(radio元素本身)
Object.defineProperty($RadioGroupObject.prototype, 'checkedItem', {
    get(){
        if (!this.items || this.items.length == 0) return undefined;
        for (const i of this.items) {
            if (i.checked) {
                return i;
            }
        }
        return undefined;
    }
});

//選定的radio項目文字
Object.defineProperty($RadioGroupObject.prototype, 'checkedItemLabelText', {
    get() {
        if (!this.items || this.items.length == 0) return undefined;
        for (const i of this.items) {
            if (i.checked) {
                let text = i.getAttribute('data-label-text');
                if (!text) text = i.value;
                return text;
            }
        }
        return undefined;
    }
});

//選項變更自訂事件名稱
Object.defineProperty($RadioGroupObject.prototype, 'changeEventName', {
    get() {
        return `${this.groupName}_change`;
    }
});

//以選項值，指定要選定的選項
$RadioGroupObject.prototype.setCheckedValue = function (value) {
    if (!this.items || this.items.length == 0) return ;
    for (const i of this.items) {
        if (i.value == value) {
            i.checked = true;
            this.notifyChange();
            return;
        }
    }
    console.log(`setCheckedValue() failed. value=${value} not found`);
}

//通知選項變更事件
$RadioGroupObject.prototype.notifyChange = function () {
    let ci = this.checkedItem;
    if (ci != this.previousCheckedItem) {
        this.previousCheckedItem = ci;
        if (this.onchange) {
            this.onchange(ci);
        }
        //$EventHub().fireEvent(this.changeEventName, ci);
    }
}

//綁定選項文字標籤(click行為連動)
$RadioGroupObject.prototype.bindLabel = function () {
    if (!this.items || this.items.length == 0) return;
    for (const i of this.items) {
        let found;
        let elm = i.nextElementSibling;
        if (elm) {
            if (elm.tagName == 'LABEL') {
                found = elm;
            } else  if (elm.classList.contains('radio-label')) {
                found = elm;
            }
        }
        if (found) {
            found.classList.remove('block');
            found.classList.add('inline');
            //keep label text
            i.setAttribute('data-label-text', found.innerText);
            //binding click behavior
            found.addEventListener('click', (e) => {
                let label = e.currentTarget;
                let radio = label.previousElementSibling;
                radio.checked = true;
                this.notifyChange();
            });
        }
    }
}

//radio選項群組
function $RadioGroup(radioName, isBindingLabel) {
    if (!radioName) {
        console.log(`$RadioUtils() failed. radioName is undefined.`);
        return undefined;
    }

    let found = $RadioGroups.find(x => x.groupName == radioName);
    if (found) return found;

    let radioGroup = new $RadioGroupObject(radioName);
    if (radioGroup) {
        $RadioGroups.push(radioGroup);
    }
    return radioGroup;
}


/*Ligher Binder*/
function $Bind(sourceId, targetId, predicate, isRealtime) {
    let source = $GetElement(sourceId);
    let target = $GetElement(targetId);
    if (!source) {
        console.log(`$Bind() failed. sourceId=${sourceId} not found.`);
        return;
    }
    if (!target) {
        console.log(`$Bind() failed. sourceId=${targetId} not found.`);
        return;
    }
    source.addEventListener('change', (e) => {
        if (!predicate || predicate()) {
            let value = GetValue(e.target);
            SetValue(target, value);
        }
    });

    if (isRealtime) {
        source.addEventListener('keydown', (e) => {
            if (!predicate || predicate()) {
                let value = GetValue(e.target);
                SetValue(target, value);
            }
        });
    }
}

/*RWD*/
//RWD變換寬度尺寸切割點
function $RWDCutPointObject() {
    this.cutPoints = [576, 768, 992, 1200, 1440, 1920,3840];
    this.levels = [1, 2, 3, 4, 5, 6, 7];
    this.levelNames = ['xs', 's', 'm', 'l', 'xl', 'xxl','xxl2'];
    this.level1 = 576;
    this.level2 = 768;
    this.level3 = 992;
    this.level4 = 1200;
    this.level5 = 1440;
    this.level6 = 1920;
    this.currentLevel = -1;
    this.isChanged = false;
}

$RWDCutPointObject.prototype.getRwdByWidth = function (width) {
    if (!width) {
        console.log(`RWD level not valid. the width argument is not given.`);
        return {level:-1,levelName:'none',widthLowBound:0,widthHighBound:0};
    }
    if (typeof width === 'string') {
        width = width.replace(/px$/, '');
        if (isNaN(width) == false) {
            console.log(`RWD level() failed. the width argument format is not valid. use pixcel in number as width argument.`);
            return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };
        }
    }
    let len = this.cutPoints.length;
    if (width < this.cutPoints[0]) return { level: 1, levelName: 'xs', widthLowBound: 0, widthHighBound: this.cutPoints[0] };
    if (width >= this.cutPoints[len - 1]) return { level: this.levels[len - 1], levelName: this.levelNames[len - 1], widthLowBound: this.cutPoints[len-1], widthHighBound: 0 };
    for (let i = 0; i < len - 1; i++ ) {
        if (width >= this.cutPoints[i] && width < this.cutPoints[i + 1]) {
            return { level: this.levels[i], levelName: this.levelNames[i], widthLowBound: this.cutPoints[i], widthHighBound: this.cutPoints[i + 1] };
        }
    }
    return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };;
}

$RWDCutPointObject.prototype.getRwdByName = function (rwdLevelName) {
    if (!rwdLevelName) return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };
    let len = this.cutPoints.length;
    for (let i = 0; i < len ; i++) {
        if (this.levelNames[i] == rwdLevelName) {
            if (i < len - 1) {
                return { level: this.levels[i], levelName: this.levelNames[i], widthLowBound: this.cutPoints[i], widthHighBound: this.cutPoints[i + 1] };
            } else {
                return { level: this.levels[len - 1], levelName: this.levelNames[len - 1], widthLowBound: this.cutPoints[len-1], widthHighBound: 0 };
            }
        }
    }
    return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };
}

$RWDCutPointObject.prototype.getRwdByLevel = function (rwdLevel) {
    if (rwdLevel <= 0) return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };
    if (rwdLevel >= this.levels[len - 1]) return { level: this.levels[len - 1], levelName: this.levelNames[len - 1], widthLowBound: this.cutPoints[len - 1], widthHighBound: 0 };
    let len = this.cutPoints.length;
    for (let i = 0; i < len; i++) {
        if (this.levels[i] == rwdLevel) {
            if (i < len - 1) {
                return { level: this.levels[i], levelName: this.levelNames[i], widthLowBound: this.cutPoints[i], widthHighBound: this.cutPoints[i + 1] };
            } else {
                return { level: this.levels[len - 1], levelName: this.levelNames[len - 1], widthLowBound: this.cutPoints[len-1], widthHighBound: 0 };
            }
        }
    }
    return { level: -1, levelName: 'none', widthLowBound: 0, widthHighBound: 0 };
}

//計算RWD尺寸級別切割點：1~7:數字越小尺寸越小，若width格式不對，傳回-1
$RWDCutPointObject.prototype.calculateRwdLevel = function (width) {
    if (!width) {
        console.log(`RWD level not valid. the width argument is not given.`);
        return -1;
    }
    if (typeof width === 'string') {
        width = width.replace(/px$/, '');
        if (isNaN(width) == false) {
            console.log(`RWD level() failed. the width argument format is not valid. use pixcel in number as width argument.`);
            return -1;
        }
    }

    width = parseInt(width);

    if (width < this.level1) {
        return 1;
    }
    if (width >= this.level6) {
        return 7;
    }
    if (width >= this.level5) {
        return 6;
    }
    if (width >= this.level4) {
        return 5;
    }
    if (width >= this.level3) {
        return 4;
    }
    if (width >= this.level2) {
        return 3;
    }
    if (width >= this.level1) {
        return 2;
    }

    return -1;
}

//RWD變換寬度尺寸切割點
function $RWDCutPoint() {
    return new $RWDCutPointObject();
}

//RWD執行境
function $RWDContext() {
    this.isChanged = false;
    this.currentLevel = -1;
}

//Lighter RWD目標物件，若傳入target，則取target寬度，否則取body寬度，做為RWD判斷條件
function $RWDTarget(target_elemenet_or_id) {
    //target
    if (!target_elemenet_or_id) {
        this.target = document.body;
    } else {
        this.target = $GetElement(target_elemenet_or_id);
        if (!this.target) { console.log(`$RWDTarget() failed. the given argument is not a valid element or id.`); }
    }
    //rwd level是否改變
    this.rwdLevelChanged = false;
}
//actualWidth
Object.defineProperty($RWDTarget.prototype, 'actualWidth', {
    get() {
        return this.target.clientWidth || this.target.style.width;
    }
});
//rwdLevel
Object.defineProperty($RWDTarget.prototype, 'rwdLevel', {
    get() {
        let level = this.target.getAttribute('data-rwd-level');
        if (!level) return -1;
        return parseInt(level);
    },
    set(level) {
        this.target.setAttribute('data-rwd-level', level);
    }
});
//rwdState
Object.defineProperty($RWDTarget.prototype, 'rwdState', {
    get() {
        return this.target.getAttribute('data-rwd-state');
    },
    set(state) {
        this.target.setAttribute('data-rwd-state', state);
    }
});
//rwdContext
Object.defineProperty($RWDTarget, 'rwdContext', {

});

//針對目標物件的RWD調整動作
$RWDTarget.prototype.action = function (actionCallback) {
    /*
    * action針對$RWDTarget，所要執行的function
    * actionCallback 定義：(t)=>{ 
    *   t:$RWDTarget物件
    * }
    */

    if (!this.target) {
        console.log(`RWD() target is undefined.`);
        return;
    }

    let ON_RWD_ACTION_FLAG_NAME = 'on-rwd-action';

    if (HasFlag(this.target, ON_RWD_ACTION_FLAG_NAME)) {
        //上一個rwd action還在執行中，故略過一次
        return;
    }

    let cutPoint = $RWDCutPoint();

    let currentLevel = cutPoint.calculateRwdLevel(this.actualWidth);
    this.rwdLevelChanged = currentLevel != this.rwdLevel;
    if (this.rwdLevelChanged) this.rwdLevel = currentLevel;
    //設flag on，若執行rwd action期間重複執行時，則略過不做
    SetFlagOn(this.target, ON_RWD_ACTION_FLAG_NAME);
    actionCallback(this);
    //after the rwd actionCallback 重新以新的actualWidth計算新的rwdLevel
    Delay(400).then(() => {
        this.rwdLevel = cutPoint.calculateRwdLevel(this.actualWidth);
        //設flag off，表示rwd action執行完畢
        SetFlagOff(this.target, ON_RWD_ACTION_FLAG_NAME);
    }).catch(err => {
        //設flag off，表示rwd action執行完畢
        SetFlagOff(this.target, ON_RWD_ACTION_FLAG_NAME);
    });
};

//依照內容區實際寬度，調整RWD顯示
$RWDTarget.prototype.contentRWD = function (rwdLevel) {
    /*
     * 若有傳入rwdLevel，則優先使用rwdLevel進行調整
     * 若無傳入rwdLevel，則計算目前的實際寬度rwdLevel
     */
    if (!this.target) {
        console.log(`RWD() target is undefined.`);
        return;
    }

    let cutPoint = $RWDCutPoint();
    //延遲400ms再計算actualWidth的rwdLevel(因為通常要等到頁面的版面變動完成後再計算，才會反應出實際寬度)
    Delay(400).then(() => {
        let currentLevel = cutPoint.calculateRwdLevel(this.actualWidth);
        //this.rwdLevelChanged = currentLevel != this.rwdLevel;
        //if (this.rwdLevelChanged) this.rwdLevel = currentLevel;

        if (!this.target.classList.contains('content-rwd')) {
            this.target.classList.add('content-rwd');
        }
        this.target.classList.remove('level-1', 'level-2', 'level-3', 'level-4', 'level-5', 'level-6');
        if (rwdLevel) {
            this.target.classList.add(`level-${rwdLevel}`);
        } else {
            if (rwdLevel == 0) {
                this.target.classList.add(`level-0`);
            } else {
                this.target.classList.add(`level-${currentLevel}`);
            }
        }
    }).catch(err => {
        console.log(`contentRWD() failed:${err}`);
    });

}

//取消內容導向的RWD效果
$RWDTarget.prototype.unsetContentRWD = function () {
    if (!this.target) {
        console.log(`RWD() target is undefined.`);
        return;
    }
    this.target.classList.remove('level-1', 'level-2', 'level-3', 'level-4', 'level-5', 'level-6');
}

//Lighter RWD目標物件 short syntax
function $RWD(target) {
    return new $RWDTarget(target);
}

//設定事件處理函式
//若有指定第2組、第3組handler時，則依照data-handler-index屬性來決定要執行哪一組handler
function $EventHandler(selector, eventName, handler1, handler2, handler3) {
    //let targets = document.querySelectorAll(selector);
    let targets = $GetElementArray(selector);
    if (!targets || targets.length == 0) return;
    //watch
    console.log(`set [${eventName}] event handler for [${selector}]. target count=${targets.length}`);
    if (!handler1) {
        console.log('[handler1] argument is missing');
        return;
    }

    if (!handler2) {
        //只有1組handler
        for (const t of targets) {
            t.addEventListener(eventName, handler1);
        }
    } else {
        //有多組handler
        let data_attr_name = `data-${eventName}-handler-index`;
        for (const t of targets) {
            t.addEventListener(eventName, (e) => {
                let index = t.getAttribute(data_attr_name);
                if (!index) index = '0';
                if (index == '0') {
                    handler1(e);
                } else if (index == '1') {
                    if (handler2) {
                        handler2(e);
                    } else {
                        console.log(`[handler2] argument is missing for the ${eventName}] event of [${selector}]`);
                        return;
                    }
                } else if (index == '2') {
                    if (handler3) {
                        handler3(e);
                    } else {
                        console.log(`[handler3] argument is missing for the ${eventName}] event of [${selector}]`);
                        return;
                    }
                } else {
                    console.log(`${data_attr_name} of [${selector}] must smaller than 3, the current value is ${index}`);
                    return;
                }
            }); 
        }
    }
}

//切換式事件處理函式
//(event_handler_index:0,1,2分別表示第1組、第2組、第3組處理函式)
function $SwapEventHandler(target, eventName, event_handler_index) {
    if (!target) {
        console.log(`$SwapEventHandler() failed. the target [${target}] is null`);
        return;
    }
    if (isNaN(event_handler_index)) {
        console.log(`$SwapEventHandler() failed. the event_handler_index must be number and between 0 and 2. the current value is [${event_handler_index}]`);
        return;
    }

    let index = parseInt(event_handler_index);
    if (index > 2) {
        console.log(`$SwapEventHandler() failed. the event_handler_index must be between 0 and 2. the current value is [${event_handler_index}]`);
        return;
    }

    let data_attr_name = `data-${eventName}-handler-index`;
    target.setAttribute(data_attr_name, event_handler_index);

}

/*Lighter Objects*/
//資料分頁bar Lighter Object
function $PagingBarObject(pagingBarContainerId) {
    this.id = pagingBarContainerId;
    this.target = $GetElement(pagingBarContainerId);
    this.currentPageNo = 1; //預設在第1頁
    this.first = undefined;
    this.moreBefore = undefined;
    this.previous = undefined;
    this.middle = undefined;
    this.next = undefined;
    this.moreAfter = undefined;
    this.last = undefined;
    this.pagingSetting = undefined;
    this.changePageHandler = undefined;
    this.changePageActionUrl = undefined;
    this.bindingTargetId = undefined;
    this.moreBeforeVisible = false;
    this.moreAfterVisible = false;

    if (!this.target) {
        console.log(`$PagingBar() failed. the given pagingBarContainerId argument is not a valid element or id.`);
    }

    //if (this.target) {
    //    this.first = this.target.querySelector('.first');
    //    this.moreBefore = this.target.querySelector('.more-before');
    //    this.previous = this.target.querySelector('.previous');
    //    this.middle = this.target.querySelector('.middle');
    //    this.next = this.target.querySelector('.next');
    //    this.moreAfter = this.target.querySelector('.more-after');
    //    this.last = this.target.querySelector('.last');
    //    //換頁事件
    //    if (this.first) this.first.addEventListener('click', (e) => { this.changePage(e); });
    //    if (this.previous) this.previous.addEventListener('click', (e) => { this.changePage(e); });
    //    if (this.middle) this.middle.addEventListener('click', (e) => { this.changePage(e); });
    //    if (this.next) this.next.addEventListener('click', (e) => { this.changePage(e); });
    //    if (this.last) this.last.addEventListener('click', (e) => { this.changePage(e); });
    //} else {
    //    console.log(`$PagingBar() failed. the given pagingBarContainerId argument is not a valid element or id.`);
    //}

}
//設定分頁參數
$PagingBarObject.prototype.set = function (pagingSetting, changePageHandler, changePageActionUrl, bindingTargetId) {
    /*
     * pagingSetting:資料分頁參數
     * changePageHandler:換頁事件處理函式
     */

    this.pagingSetting = pagingSetting;
    if (changePageHandler) {
        this.changePageHandler = changePageHandler;
    }
    if (changePageActionUrl) {
        this.changePageActionUrl = changePageActionUrl;
    }
    if (bindingTargetId) {
        this.bindingTargetId = bindingTargetId;
    }

    //建立分頁按鈕
    let pageCount = 0;
    if (pagingSetting && pagingSetting.pageCount) {
        pageCount = pagingSetting.pageCount;
    }
    this.buildButtons(pageCount);

    if (pagingSetting && pagingSetting.pageCount > 5) {
        this.moreAfterVisible = true;
    }

    if (pagingSetting && pagingSetting.pageNo) {
        this.setButtonState(pagingSetting.pageNo);
    } else {
        this.setButtonState(1);
    }

    return this;
}

//建立分頁按鈕
$PagingBarObject.prototype.buildButtons = function (pageCount) {
    if (this.target) {
        /*
         * 有到達該頁數->加入按鈕；反之，移除按鈕
         */
        if (pageCount && pageCount > 0) {
            this.createButton('first', pageCount, 1);
            //若大於5頁，加入....
            this.createMoreSpan('moreBefore', 'more-before', pageCount);
            this.createButton('previous', pageCount, 2);
            this.createButton('middle', pageCount, 3);
            this.createButton('next', pageCount, 4);
            //若大於5頁，加入....
            this.createMoreSpan('moreAfter', 'more-after', pageCount);
            if (pageCount >= 5) {
                this.createButton('last', pageCount, pageCount);
            }

            //if (pageCount >= 1) {
            //    this.first = this.target.querySelector('.first');
            //    if (!this.first) {
            //        this.first = this.createButton('first', '1');
            //        this.target.appendChild(this.first);
            //    }
            //}
            ////若大於5頁，加入....
            //this.moreBefore = this.target.querySelector('.more-before');
            //if (pageCount > 5) {
            //    if (!this.moreBefore) {
            //        this.moreBefore = this.createMoreSpan('more-before');
            //        this.target.appendChild(this.moreBefore);
            //    }
            //} else {
            //    if (this.moreBefore) {
            //        this.moreBefore.remove();
            //        this.moreBefore = null;
            //    }
            //}

            //this.previous = this.target.querySelector('.previous');
            //if (pageCount >= 2) {
            //    if (!this.previous) {
            //        this.previous = this.createButton('previous', '2');
            //        this.target.appendChild(this.previous);
            //    }
            //} else {
            //    if (this.previous) {
            //        this.previous.remove();
            //        this.previous = null;
            //    }
            //}

            //if (pageCount >= 3) {
            //    this.middle = this.target.querySelector('.middle');
            //    if (!this.middle) {
            //        this.middle = this.createButton('middle', '3');
            //        this.target.appendChild(this.middle);
            //    }
            //}

            //if (pageCount >= 4) {
            //    this.next = this.target.querySelector('.next');
            //    if (!this.next) {
            //        this.next = this.createButton('next', '4');
            //        this.target.appendChild(this.next);
            //    }
            //}

            // //若大於5頁，加入....
            //if (pageCount > 5) {
            //    this.moreAfter = this.target.querySelector('.more-after');
            //    if (!this.moreAfter) {
            //        this.moreAfter = this.createMoreSpan('more-after');
            //        this.target.appendChild(this.moreAfter);
            //    }
            //}

            //if (pageCount >= 5) {
            //    this.last = this.target.querySelector('.last');
            //    if (!this.last) {
            //        this.last = this.createButton('last', `${pageCount}`);
            //        this.target.appendChild(this.last);
            //    }
            //}
            
        } else {
            //清除按鈕
            this.target.innerHTML = '';
            this.first = null;
            this.previous = null;
            this.middle = null;
            this.next = null;
            this.last = null;
            this.moreBefore = null;
            this.moreAfter = null;
        }


        //換頁事件(標記flag，防止重複綁定事件)
        if (this.first && !GetFlag(this.first, 'hasEvent')) {
            this.first.addEventListener('click', (e) => { this.changePage(e); });
            SetFlagOn(this.first,'hasEvent');
        } 

        if (this.previous && !GetFlag(this.previous, 'hasEvent')) {
            this.previous.addEventListener('click', (e) => { this.changePage(e); });
            SetFlagOn(this.previous, 'hasEvent');
        } 

        if (this.middle && !GetFlag(this.middle, 'hasEvent')) {
            this.middle.addEventListener('click', (e) => { this.changePage(e); });
            SetFlagOn(this.middle, 'hasEvent');
        } 

        if (this.next && !GetFlag(this.next, 'hasEvent')) {
            this.next.addEventListener('click', (e) => { this.changePage(e); });
            SetFlagOn(this.next, 'hasEvent');
        } 

        if (this.last && !GetFlag(this.last, 'hasEvent')) {
            this.last.addEventListener('click', (e) => { this.changePage(e); });
            SetFlagOn(this.last, 'hasEvent');
        } 

    } else {
        console.log(`$PagingBar() failed. The target element not found. Check if the pagingBarContainerId argument is a valid element or id.`);
    }
}

//產生個別按鈕
$PagingBarObject.prototype.createButton = function (btnName, pageCount, btnPageNo) {
    /*
     * 若按鈕存在，則用之，若不存在，則產生
     * 若頁數未到達按鈕頁碼，則移除之
     */
    this[btnName] = this.target.querySelector(`.${btnName}`);
    if (pageCount >= btnPageNo) {
        if (!this[btnName]) {
            let btn = document.createElement('button');
            btn.classList.add(btnName);
            btn.setAttribute('value', btnPageNo);
            btn.innerText = btnPageNo;
            this[btnName] = btn;
            this.target.appendChild(this[btnName]);
        }
    } else {
        if (this[btnName]) {
            this[btnName].remove();
            this[btnName] = null;
        }
    }

}

//產生...按鈕(大於5頁時需要)
$PagingBarObject.prototype.createMoreSpan = function (spanName, spanClass, pageCount) {
    this[spanName] = this.target.querySelector(`.${spanClass}`);
    if (pageCount > 5) {
        if (!this[spanName]) {
            let span = document.createElement('span');
            span.classList.add(spanClass);
            if (spanClass == 'more-before') {
                span.classList.add('hide');
            }
            span.innerText = '....';
            this[spanName] = span;
            this.target.appendChild(this[spanName]);
        }
    } else {
        if (this[spanName]) {
            this[spanName].remove();
            this[spanName] = null;
        }
    }

}

//換頁事件
$PagingBarObject.prototype.changePage = function (e) {
    if (!this.changePageHandler) {
        console.log('changePage() failed. changePageHandler is undefined. use set() method to provide a valid changePageHandler argument');
        return;
    }
    e.stopPropagation(); //停止event bubbling
    e.preventDefault(); //取消預設動作
    let btn = e.currentTarget; 
    let pageNo = parseInt(btn.value);  //頁碼

    if (this.pagingSetting) {
        this.pagingSetting.pageNo = pageNo;
        this.changePageHandler(this.pagingSetting, this.changePageActionUrl, this.bindingTargetId);
    } else {
        this.changePageHandler({pageNo:pageNo});
    }

    this.setButtonState(pageNo);
}

//設定按鈕狀態
$PagingBarObject.prototype.setButtonState = function (pageNo) {

    if (!this.pagingSetting.pageCount || this.pagingSetting.pageCount == 0) return;
    if (!pageNo) pageNo = 1;
    if (isNaN(pageNo)) pageNo = 1;
    if (typeof pageNo === 'string') pageNo = parseInt(pageNo);

    //調整paging-bar
    //移除上一個選定的頁碼效果
    if (this.currentPageNo == this.first.value) {
        this.first.classList.remove('current-page');
    } else if (this.currentPageNo == this.previous.value) {
        this.previous.classList.remove('current-page');
    } else if (this.currentPageNo == this.middle.value) {
        this.middle.classList.remove('current-page');
    } else if (this.currentPageNo == this.next.value) {
        this.next.classList.remove('current-page');
    } else if (this.currentPageNo == this.last.value) {
        this.last.classList.remove('current-page');
    }

    //設定新的選定的頁碼效果
    if (this.pagingSetting.pageCount <= 5) {
        switch (pageNo) {
            case 1:
                this.first.classList.add('current-page');
                break;
            case 2:
                this.previous.classList.add('current-page');
                break;
            case 3:
                this.middle.classList.add('current-page');
                break;
            case 4:
                this.next.classList.add('current-page');
                break;
            case 5:
                this.last.classList.add('current-page');
                break;
        }
    } else {
        if (pageNo == this.first.value) {
            this.first.classList.add('current-page');
            Hide(this.moreBefore);
            this.moreBeforeVisible = false;
            if (this.pagingSetting.pageCount > 5) {
                Show(this.moreAfter);
                this.moreAfterVisible = true;
            }

            this.next.value = 4;
            this.next.innerText = 4;
            this.middle.value = 3;
            this.middle.innerText = 3;
            this.previous.value = 2;
            this.previous.innerText = 2;

        } else if (pageNo == this.previous.value) {
            if (pageNo > 2) {
                this.next.value = pageNo + 1;
                this.next.innerText = pageNo + 1;
                this.middle.value = pageNo;
                this.middle.innerText = pageNo;
                this.previous.value = pageNo - 1;
                this.previous.innerText = pageNo - 1;
                this.middle.classList.add('current-page');
                if (pageNo == 3) {
                    Hide(this.moreBefore);
                    this.moreBeforeVisible = false;
                }
            }
            if (pageNo == 2) {
                this.previous.classList.add('current-page');
            }
            if (pageNo == this.pagingSetting.pageCount - 3) {
                Show(this.moreAfter);
                this.moreAfterVisible = true;
            }
        } else if (pageNo == this.middle.value) {
            this.middle.classList.add('current-page');
        } else if (pageNo == this.next.value) {
            if (pageNo < this.pagingSetting.pageCount - 1) {
                this.next.value = pageNo + 1;
                this.next.innerText = pageNo + 1;
                this.middle.value = pageNo;
                this.middle.innerText = pageNo;
                this.previous.value = pageNo - 1;
                this.previous.innerText = pageNo - 1;
                this.middle.classList.add('current-page');
                if (pageNo == this.pagingSetting.pageCount - 2) {
                    Hide(this.moreAfter);
                    this.moreAfterVisible = false;
                }
            }
            if (pageNo == this.pagingSetting.pageCount - 1) {
                this.next.classList.add('current-page');
            }
            if (pageNo > 3 && this.moreBeforeVisible == false) {
                Show(this.moreBefore);
                this.moreBeforeVisible = true;
            }

        } else if (pageNo == this.last.value - 1) {
            this.next.classList.add('current-page');
            Hide(this.moreAfter);
            this.moreAfterVisible = false;
            if (this.pagingSetting.pageCount > 5) {
                Show(this.moreBefore);
                this.moreBeforeVisible = true;
            }

            this.next.value = pageNo;
            this.next.innerText = pageNo;
            this.middle.value = pageNo - 1;
            this.middle.innerText = pageNo - 1;
            this.previous.value = pageNo - 2;
            this.previous.innerText = pageNo - 2;

        } else if (pageNo == this.last.value) {
            this.last.classList.add('current-page');
            Hide(this.moreAfter);
            this.moreAfterVisible = false;
            if (this.pagingSetting.pageCount > 5) {
                Show(this.moreBefore);
                this.moreBeforeVisible = true;
            }

            this.next.value = this.pagingSetting.pageCount - 1;
            this.next.innerText = this.pagingSetting.pageCount - 1;
            this.middle.value = this.pagingSetting.pageCount - 2;
            this.middle.innerText = this.pagingSetting.pageCount - 2;
            this.previous.value = this.pagingSetting.pageCount - 3;
            this.previous.innerText = this.pagingSetting.pageCount - 3;

        }
    }
    //更新新選定的頁碼
    this.currentPageNo = pageNo;
}

//刷新
$PagingBarObject.prototype.refresh = function () {
    if (this.pagingSetting) {
        changePageHandler(this.pagingSetting);
    } else {
        changePageHandler({pageNo: 1 });
    }
}

//資料分頁bar集合
let $PagingBars = []; /***Global***/
//資料分頁bar Lighter Object (shorthand)
function $PagingBar(pagingBarContainerId) {
    let index = $PagingBars.findIndex(x => x.id == pagingBarContainerId);
    let barObject;
    if (index < 0) {
        barObject = new $PagingBarObject(pagingBarContainerId);
        let length = $PagingBars.push(barObject);
    } else {
        barObject = $PagingBars[index];
    }
    return barObject;
}


//取得物件的型別資訊json
function _GetTypeName(obj) {
    let typeInfo = {
        constructorName: obj.constructor.name,
        typeName: typeof obj
    };
    return JSON.stringify(typeInfo);
}

//將逗號分隔的id或name拆解成陣列(並去掉前綴的#號)
function SplitToIdOrNameArray(ids_or_names) {
    if (!ids_or_names) return [];
    let ids = ids_or_names.split(',').map(x=>x.trim());
    let cleaned = ids.map(x => x.replace(/^#/, ''));
    return cleaned;
}

//將逗號分隔的selector拆解成陣列
function SplitToSelectors(selectors) {
    if (!selectors) return [];
    let arr = selectors.split(',').map(x => x.trim());
    return arr;
}

//轉換成布林值
function ConvertToBoolean(value) {
    if (!value) return false;
    let strValue = value.toString().toUpperCase();
    let trueValues = ['TRUE,FALSE', 'TRUE', 'YES', 'ON', 'Y', '1', 'T'];
    return trueValues.indexOf(strValue) >= 0;
}

//將布林值轉換成'0'或'1'字元
function ConvertToCharBool(value) {
    if (!value) return '0';
    let strValue = value.toString().toUpperCase();
    let trueValues = ['TRUE,FALSE', 'TRUE', 'YES', 'ON', 'Y', '1', 'T'];
    let charBool = (trueValues.indexOf(strValue) >= 0) ? '1' : '0';
    return charBool;
}

//轉換json字串成js物件
function ConvertToJSObject(json) {
    if (!json) return undefined;
    let jsObj;

    if (typeof json === 'string') {
        //若傳入字串則轉成 js object
        try {
            jsObj = JSON.parse(json);
        } catch (e) {
            console.log(`convert to json object failed. original json string=${json}`);
            console.log(`error=${e}`);
            return undefined;
        }
    } else {
        //若傳入 js object or array 則不用轉換
        if (typeof json === 'object' && json !== null) {
            jsObj = json;
        }
    }

    if (!jsObj) console.log('convert to javascript object failed. the json argument may not have a valid json format. json arg=' + json);

    return jsObj;
}

//設定元素或容器範圍是否可編輯
function $SetEditable(element_or_id, isEditable) {
    let elm = $GetElement(element_or_id);
    if (!elm) return;
    //if form element then recursive for each fieldset
    if (elm.tagName.toLowerCase() == 'form') {
        let fieldsets = elm.querySelectorAll('fieldset');
        let buttons = elm.querySelectorAll('button');
        if (fieldsets) {
            for (const fset of fieldsets) {
                $SetEditable(fset, isEditable);
            }
        }
        if (buttons) {
            for (const btn of buttons) {
                $SetEditable(btn, isEditable);
            }
        }        
    }

    const disabled = elm.hasAttribute('disabled')
    if (isEditable) {
        //const firstInput = elm.querySelector('input')
        if (disabled) {
            elm.removeAttribute('disabled')
        }
        //if (firstInput) firstInput.focus()
    }
    else {
        if (!disabled) {
            elm.setAttribute('disabled', 'disabled')
        }
    }
}

//設定啟用物件
function $SetEnable(element_or_id, enableState) {
    /*
     * enableState: 啟用狀態(true/false),預設true
     */
    let enable = true;
    if (enableState !== undefined) enable = enableState;
    $SetEditable(element_or_id, enable);
}

//設定停用物件
function $SetDisable(element_or_id, disableState) {
    /*
     * diableState: 停用狀態(true/false), 預設true
     */
    let disable = true;
    if (disableState !== undefined) disable = disableState;
    $SetEditable(element_or_id, !disable);
}

//鎖定
function $Lock(element_or_id, timeout) {
    /*
     * timeout: 要鎖定多久(單位:秒)，若未給值，則鎖定直到手動解開鎖定
     */
    let elm = $GetElement(element_or_id);
    if (!elm) {
        console.log(`$Lock() failed. the element_or_id=${element_or_id} does not exist.`);
        return;
    }
    elm.setAttribute('data-locked','1');
    $SetDisable(elm);
    if (timeout && timeout != 0 && timeout != '0') {
        window.setTimeout(() => {
            let locked = elm.getAttribute('data-locked');
            if (locked == '1') {
                $Unlock(elm);
            }
        }, timeout * 1000);
    }
}

//解開鎖定
function $Unlock(element_or_id) {
    let elm = $GetElement(element_or_id);
    if (!elm) {
        console.log(`$Unlock() failed. the element_or_id=${element_or_id} does not exist.`);
        return;
    }
    elm.setAttribute('data-locked', '0');
    $SetEnable(elm);
}

//切換元素或容器範圍的可編輯狀態
function _$ToggleEditable(element_or_id) {
    let elm = $GetElement(element_or_id);
    if (!elm) return;
    const disabled = elm.hasAttribute('disabled')
    const firstInput = elm.querySelector('input')
    if (disabled) {
        elm.removeAttribute('disabled')
        if (firstInput) firstInput.focus()
    }
    else {
        elm.setAttribute('disabled', 'disabled')
    }
}

//將data model物件的值套用至表單或容器中的欄位
function $ApplyModel(jsonData_or_jsObject, form_or_container_id, target_ids_or_names) {
    /*
     * jsonData_or_jsObject: 包含欄位資料的json data或js object
     * form_or_container_id: 要套用的表單或容器元素
     * target_ids_or_names: (字串)要套用的欄位id或name(若略此參數，則套用全部欄位)，以逗號分隔多個id
     */
    let modelObj = ConvertToJSObject(jsonData_or_jsObject);
    if (!modelObj) {
        console.log(`$ApplyModel() failed. modelObj is not defined`);
    }
    let fm = $GetElement(form_or_container_id);
    if (!fm) {
        console.log(`$ApplyModel() failed. form or container [${form_or_container_id}] not existed`);
    }

    let keys; //目標欄位名稱集合
    if (!target_ids_or_names) {
        keys = Object.keys(modelObj);  //全部欄位
    } else {
        keys = SplitToIdOrNameArray(target_ids_or_names); //特定欄位
    }

    keys.forEach((k) => {
        //先找id，若無，再找name
        let elm = fm.querySelector('#' + k);
        if (!elm) {
            elm = fm.querySelector(`[data-field="${k}"]`);
        }

        if (elm) {
            SetValue(elm, modelObj[k]);
        } else{
            let elmList = fm.querySelectorAll(`[name="${k}"]`);
            if (elmList && elmList.length > 0) {
                if (elmList.length > 1) { //multiple element
                    //處理多值情況
                    console.log(`[${k}] 是多值欄位`);
                    //to do ...

                } else { //single element
                    elm = elmList[0];
                    SetValue(elm, modelObj[k]);
                }
            } else {
                console.log(`warning: $ApplyModel() element for field name[${k}] not found`);
                return;
            }
        }

    });
}

//將表單或容器中的欄位值，回存至data model物件
function $UpdateModel(modelObj, form_or_container_id, acceptNullModel) {
    /*
     * modelObj: data model物件
     * form_or_container_id: 要套用的表單或容器元素
     * target_ids_or_names: (字串)要套用的欄位id或name(若略此參數，則套用全部欄位)，以逗號分隔多個id
     */

    if (!modelObj) {
        if (!acceptNullModel) {
            console.log(`warning: $UpdateModel() modelObj is undefined`);
        }
        modelObj = {};
    }
    let fm = $GetElement(form_or_container_id);
    if (!fm) {
        console.log(`$UpdateModel() failed. form or container [${form_or_container_id}] not existed.`);
        return undefined;
    }

    let inputs = fm.querySelectorAll('[data-field]');
    if (inputs && inputs.length > 0) {
        for (let elm of inputs) {
            let field_name = elm.getAttribute('data-field');
            //(1)get value object with extra data attribute
            let valueObj = GetValueText(elm,true);
            modelObj[field_name] = valueObj.value;
            //(2)extra data for select element
            if (elm.tagName == 'SELECT') {
                if (valueObj.extraNames) {
                    let nameArr = valueObj.extraNames.split(',').map(x => x.trim());
                    let valueArr = valueObj.extraValues.split(',').map(x => x.trim());
                    if (nameArr.length == valueArr.length) {
                        for (let i = 0; i < nameArr.length; i++) {
                            modelObj[nameArr[i]] = valueArr[i];
                        }
                    } else {
                        console.log(`warning: $UpdateModel extraNames.length != extraValues.length`);
                    }
                }
            }
         }
        return modelObj;

    } else {
        console.log(`$UpdateModel() failed. input elements with [data-field] attribute not found.`);
        return undefined;
    }

}

//將表單或容器中的欄位值，回存至data model物件
function $RetriveModel(form_or_container_id) {
    return $UpdateModel(null, form_or_container_id, true)
}

//監測檢核失敗欄位變更事件
function $MonitorValidateErrorFieldChange(e) {
    ClearValidationHint(null, e.currentTarget);
}

//設定form-item項目的資料檢核提示訊息和框線
function SetValidationHint(form_or_id,hintMessages,element_or_id_or_name) {
    /*
     * form_or_id:form元素或id
     * hintMessages: 提示訊息，若為多欄位則傳入json，格式如下：
     * [
     *  {"propertyName":"id1","message":"message1"},
     *  {"propertyName":"id2","message":"message2"},
     *  {"propertyName":"id3","message":"message3"}
     * ]
     * 其中id對應到form-input欄位的id屬性，message是提示訊息
     * 注意：若欄位元素未設定id屬性，此功能無作用
     * element_or_id_or_name:(若有指定)表示只針對指定的欄位元素，優先比對id，其次比對name
     */

    let form, elm;
    form = $GetElement(form_or_id);

    if (form) {
        //設定model-error flag
        SetFlagOn(form, 'model-error');
    }

    //針對單一欄位
    if (element_or_id_or_name) {
        elm = $GetElement(element_or_id_or_name);
        //若id找無，且有form時，則用name找
        if (!elm && form) {
            let element_key = element_or_id_or_name;
            elm = form[element_key];
        }
        if (elm) {
            elm.classList.add('form-input-highlight');
            let parent = elm.parentElement;
            if (parent) {
                parent.setAttribute('data-hint', hintMessages);
            }
            //設定監測事件處理
            elm.addEventListener('change', $MonitorValidateErrorFieldChange);

            //調整可視
            elm.scrollIntoView({behavior:"smooth",block:"center",inline:"center"});
        }

        return;
    }

    if (!form) {
        console.log(`SetValidationHint() failed: ${form_or_id} not found.`);
        return;
    }

    if (!hintMessages) return;
    let hintArr = ConvertToJSObject(hintMessages);

    if (!hintArr) {
        console.log(`convert to jsObject failed. hintMessages=${hintMessages}`);
        return;
    }

    if (hintArr.length == 0) {
        console.log(`warning: length of hintMessages is 0`);
        return;
    }

    //針對全部欄位，包含(1)有標註class=form-input或class=field的欄位 (2)hidden欄位
    let formInputs = form.querySelectorAll(".field,.form-input,input[type=hidden]");
    if (formInputs && formInputs.length > 0) {
        let firstFoundElm; //第1個顯示錯誤訊息的欄位
        for (const item of formInputs) {
            //以form-input欄位的id查找對應的提示訊息
            if (hintArr.length > 0) {
                let hintIndex = hintArr.findIndex(x => x.propertyName == item.id);
                if (hintIndex < 0 && item.name) { //若id找無，找name
                    hintIndex = hintArr.findIndex(x => x.propertyName == item.name);
                }
                if (hintIndex < 0) continue;

                if (!firstFoundElm) firstFoundElm = item;

                let hint = hintArr[hintIndex];
                let parent = item.parentElement;
                if (hint && hint.message) {
                    //form-input項目加上紅色外框
                    item.classList.add('form-input-highlight');
                    //設定form-input的上層元素form-item下方顯示提示文字
                    if (parent) {
                        parent.setAttribute('data-hint', hint.message);
                    }
                    //設定監測事件處理
                    item.addEventListener('change', $MonitorValidateErrorFieldChange);
                }
                else {
                    //若無提示訊息，則給一個全形空白，使欄位垂直對齊
                    if (parent) {
                        parent.setAttribute('data-hint', '');
                        parent.classList.remove('form-input-highlight');
                    }
                }

                //每次顯示hint之後即可移除該項
                hintArr.splice(hintIndex, 1);
            }
        }

        if (firstFoundElm) {
            //調整可視
            firstFoundElm.scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
        }

    }

    if (hintArr && hintArr.length > 0) {
        //有剩餘沒對應到欄位的檢核訊息
        let msg = hintArr.map(x => `${x.propertyName}:${x.message}`).join();
        console.log(`SetValidationHint() missing fields: ${msg}`);
    }
}

//清除form-item項目的資料檢核提示訊息和框線
function ClearValidationHint(form_or_id, element_or_id_or_name) {
    /*
     * form_or_id:表單物件或id，若有提供，則只處理此表單中的欄位，若無提供，則處理整個頁面的欄位
     * element_or_id_or_name:(若有指定)表示只針對指定的欄位元素
     */

    let form; //表單
    let elm; //單一元素
    let formItems; //表單元素

    //單一欄位
    if (element_or_id_or_name) {
        elm = $GetElement(element_or_id_or_name);
        //若id找無，且有form時，則用name找
        if (!elm) {
            if (form_or_id) {
                form = $GetElement(form_or_id);
            }
            if (form) {
                let element_key = element_or_id_or_name;
                elm = form[element_key];
            }
        }
        if (elm) {
            if (elm.form) {
                form = elm.form;
            } else {
                form = FindParent(elm, 'form', 5);
            }
            let parent = elm.parentElement; //hint是加在上一層元素的尾端
            if (parent) {
                formItems = [parent];
            }
        } else {
            console.log(`element_id:${element_or_id} not found.`);
            return;
        }
    } else {
        //全部欄位
        if (form_or_id) {
            //特定表單
            if (!form) {
                form = $GetElement(form_or_id);
                if (!form) {
                    console.log(`form_id:${form_id} not found.`);
                    return;
                }
            } 
            formItems = form.querySelectorAll(".form-item");
        } else {
            //全部表單
            formItems = document.querySelectorAll(".form-item");
        }
    }

    //取消model-error flag
    SetFlagOff(form, 'model-error');

    if (formItems) {
        for (const item of formItems) {
            //form-input項目去除紅色外框
            let formInput = item.querySelector('.field,.form-input');
            if (formInput) {
                formInput.classList.remove('form-input-highlight');
                //重繪元素
                let elmType = formInput.getAttribute('type');
                if (elmType && elmType == 'text') {
                    formInput.value = formInput.value + ' ';
                    formInput.value = formInput.value.trim();
                }
                let redrawHack = formInput.offsetHeight;
                //移除監測事件處理
                formInput.removeEventListener('change', $MonitorValidateErrorFieldChange);
            }
            //form-item項目去除下方提示文字
            item.setAttribute('data-hint', '');

        }
    }
}

//設定按鈕的命令名稱
function $SetButtonCommand(button_or_id, commandName) {
    let btn = $GetElement(button_or_id);
    if (!btn) return;
    btn.setAttribute('data-command',commandName);
}

//取得按鈕的命令名稱
function $GetButtonCommand(button_or_id) {
    let btn = $GetElement(button_or_id);
    if (!btn) return '';
    let cmd = btn.getAttribute('data-command');
    if (!cmd) cmd = '';
    return cmd;
}

//移除指定的陣列元素
function RemoveArrayElement(arr, element_or_predicate) {
    /*
     * arr:陣列
     * element_or_predicate:要移除的元素值，或判斷式
     */
    if (!arr || arr.length == 0) return;
    let index = -1;
    if (element_or_predicate.constructor.name === 'Function') {
        //predicate
        let predicate = element_or_predicate;
        index = arr.findIndex(predicate);
    } else {
        //element
        let element = element_or_predicate;
        index = arr.indexOf(element);
    }

    if (index >= 0) arr.splice(index, 1);
}

//選定的項目
function $SelectedItem(select_element_or_id) {
    /*
     * select_element_or_id: <select>元素或其id
     */
    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) return undefined;
    let selectedIndex = select_element.selectedIndex;
    if (selectedIndex < 0) return undefined;

    let s = new $SelectedItemObject();
    s.option = select_element.options[selectedIndex];
    s.index = selectedIndex;
    s.text = s.option.text;
    s.value = s.option.value;
    s.hasExtraData = false;
    s.extraData = {};
    if (s.option.hasAttribute('data-extra-name')) {
        let extraDataName = s.option.getAttribute('data-extra-name');
        let extraDataValue = s.option.getAttribute('data-extra-value');
        s.extraData[extraDataName] = extraDataValue;
        s.hasExtraData = true;
    }
    return s;
}

function $SelectedItemObject() {
    this.option;
    this.index;
    this.text;
    this.value;
    this.hasExtraData;
    this.extraData = {};
}

//選定選項值
function $Select(select_element_or_id, value) {
    /*
    * select_element_or_id: <select>元素或其id
    * value: 值(若有給此參數則是設定選項值，若無給此參數則是取得選項值)
    */
    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) return '';



    if (!value) {
        //get value
        if (select_element.selectedIndex < 0) return '';
        return select_element.options[select_element.selectedIndex].value;
    } else {
        //set value
        let options = select_element.options;
        let index = -1;
        for (var i = 0; i < options.length; i++) {
            if (options[i].value == value) {
                index = i;
                break;
            }
        }
        select_element.selectedIndex = index;
    }
}

//取得或設定選項值
function $SelectValue(select_element_or_id, value) {
    /*
     * select_element_or_id: <select>元素或其id
     * value: 值(若有給此參數則是設定選項值，若無給此參數則是取得選項值)
     */
    let select_element = $GetElement(select_element_or_id,'select');
    if (!select_element) return '';

    if (value == '' || value) {
        //set value
        let options = select_element.options;
        let index = -1;
        for (var i = 0; i < options.length; i++) {
            if (options[i].value == value) {
                index = i;
                break;
            }
        }
        select_element.selectedIndex = index;
    } else {
        //get value
        if (select_element.selectedIndex < 0) return '';
        return select_element.options[select_element.selectedIndex].value;
    }

}
//取得或設定選項文字
function $SelectText(select_element_or_id, text) {
    /*
     * select_element_or_id: <select>元素或其id
     * text: 文字(若有給此參數則是設定選項文字，若無給此參數則是取得選項文字)
     */
    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) return '';

    if (!text) {
        //get text
        if (select_element.selectedIndex < 0) return '';
        return select_element.options[select_element.selectedIndex].text;
    } else {
        //set selected text
        let options = select_element.options;
        let index = -1;
        for (var i = 0; i < options.length; i++) {
            if (options[i].text == text) {
                index = i;
                break;
            }
        }
        select_element.selectedIndex = index;
    }
}
//清除select選項
function $SelectClear(select_element_or_id) {
    /*
     * select_element_or_id: <select>元素或其id
     */
    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) return;

    let length = select_element.options.length;
    while (length > 0) {
        select_element.remove(length-1);
        length = select_element.options.length;
    }   
}
//select選項中是否包含指定文字
function $SelectContainsOptionText(select_element_or_id, text, allowEmpty) {
    /*
     * select_element_or_id: <select>元素或其id
     * text: 選項文字
     * allowEmpty: 是否容許空白項目
     */

    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) {
        console.log(`$SelectContainsOptionText() failed. select element [${select_element_or_id}] not exist.`);
        return false;
    }

    if (text) text = text.trim();

    if (!allowEmpty && !text) {
        return false;
    }

    let options = select_element.options;
    let count = options.length;
    let found = false;
    for (let i = 0; i < count; i++) {
        if (options[i].text == text) {
            found = true;
            break;
        }
    }

    return found;
}

//select選項中是否包含指定值
function $SelectContainsOptionValue(select_element_or_id, value, allowEmpty) {
    /*
     * select_element_or_id: <select>元素或其id
     * value: 選項值
     * allowEmpty: 是否容許空白項目
     */

    let select_element = $GetElement(select_element_or_id, 'select');
    if (!select_element) {
        console.log(`$SelectContainsOptionText() failed. select element [${select_element_or_id}] not exist.`);
        return false;
    }

    if (value) value = value.trim();

    if (!allowEmpty && !value) {
        return false;
    }

    let options = select_element.options;
    let count = options.length;
    let found = false;
    for (let i = 0; i < count; i++) {
        if (options[i].value == value) {
            found = true;
            break;
        }
    }

    return found;
}

//找出屬性attributeName的值=matchValue的node元素
function findNodeListElement(nodeList, predicate) {
    if (!nodeList) {
        console.log('findNode() warning: nodeList is null or undefined.');
        return undefined;
    }

    if (nodeList.length == 0) {
        console.log('findNode() warning: nodeList contains no elements.');
        return undefined;
    }

    try {
        for (let node of nodeList) {
            if (predicate(node)) {
                return node;
            }
        }
    } catch (e) {
        console.log(e);
        return undefined;
    }
    return undefined;
}

//將json文字參數轉成json物件
function parseToJsonObject(jsonString) {
    /*
     * 當jsonString符合{name:value}的json格式時，傳回parse後的物件
     * 當jsonString本身就是object type時，直接假設傳入的就是json物件
     * 當傳入其他type的參數，則傳回undefined
     */
    if (!jsonString) {
        console.log(`warning:jsonString is null or undefined`);
        return undefined;
    }

    //檢核imgSrcArgs，轉成json物件
    let obj;
    if (typeof jsonString == 'string') {
        try {
            obj = JSON.parse(jsonString); //array
        } catch (e) {
            console.log('error: JSON.parse(jsonString) failed.');
            console.log(jsonString);
            return undefined;
        }
    }
    else if (typeof jsonString == 'object') {
        obj = jsonString;
    } else {
        console.log('warning: invalid jsonString format.');
        console.log(imgSrcArgs);
        return undefined;
    }

    return obj;
}

//載入圖片(單個)
function loadImage(selector, imgSrc) {
    /*
     * selector: CSS selector用以選定要載入圖片的img tag
     * imgSrc: 圖片源url
     */
    let img = document.querySelector(selector);
    if (img) img.src = imgSrc;
}

//載入緩載圖片(單個或多個)
function loadLazyImages(selector) {
    /*
     * 依照img tag所設定的 data-lazy-src屬性值載入圖片
     * selector: CSS selector用以選定要載入圖片的img tag
     */
    let imgs = document.querySelectorAll(selector);
    if (!imgs || imgs.length == 0) return;
    for (let img of imgs) {
        let src = img.getAttribute("data-lazy-src");
        img.src = src;
    }
}

//載入圖片(至符合的id的img tag)(單個或多個)
function loadImageCollectionById(selector, imgSrcJson) {
    /*
     * selector: CSS selector用以選定要載入圖片的img tags
     * imgSrcJson: 圖片源參數(json)，格式如下：
     * {
     *  imgSrcs:[
     *      {id:"imgId-1",src="imgUrl-1"},
     *      {id:"imgId-2",src="imgUrl-2"},...
     *  ]
     * }
     */

    let jsonObj = parseToJsonObject(imgSrcJson);
    if (!jsonObj) return;

    let args = jsonObj.imgSrcs; //array

    //套用圖片源
    let imgs = document.querySelectorAll(selector);
    let imgCount = (imgs) ? imgs.length : 0;
    let argCount = (args) ? args.length : 0;
    if (imgCount > 0 && argCount > 0) {
        //依參數中的id，找出id符合的img，指定其scr
        args.forEach(arg => {
            let img = findNodeListElement(imgs, x => (x.id) && (x.id == arg.id));
            if (img) {
                img.src = arg.src;
            }
        });
    }
}

//載入圖片(單個或多個)
function loadImageCollection(selector, imgSrcs) {
    /*
     * 依序將imgSrcs的圖片源，載入selector所指定的img tags
     * selector: CSS selector用以選定要載入圖片的img tags
     * imgSrcs: 圖片源參數(url字串，若多組用逗號分隔)
    */
    if (!imgSrcs) return;
    let imgs = document.querySelectorAll(selector);
    if (!imgs) return;

    let srcs = imgSrcs.split(',').map(x=>x.trim());
    let i = 0;
    for (const img of imgs) {
        if (i >= srcs.length) break;
        img.src = srcs[i];
        i++;
    }
}

//以GET取得部分更新頁面內容的json回傳值
async function getFetchJson(url) {
    /*
     * url:要呼叫的Url
     * 格式：/{controller}/{action}/{?parameter1=value1&parameter2=value2}
    */

    /*
     * 回傳json物件格式：
     *  {
     *      success:(bool:是否成功),
     *      statusCode: Http Status Code(server端自定義),
     *      message:結果訊息,
     *      data:內容/資料物件,
     *      popup:(bool:是否跳窗顯示訊息)
     *  }
     */

    let json = await fetch(url, {
        method: 'GET',
        cache: 'no-cache',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    })

        .then((resp) => {
            if (resp.ok) {
                return resp.json();
            } else {
                return { success: false, statusCode: resp.status, message: `http error:${resp.status} ${resp.statusText}`, data: '', metaData:'', popup: false };
                //或是throw給後面catch()去處理
                //throw new Error('Http error:' + resposne.status);
            }
        })
        .catch((err) => {
            //一律回應http 400:bad request
            let errMsg = (err.message) ? err.message : 'bad request';
            let httpStatusCode = (err.statusCode) ? err.statusCode : 400;
            return { success: false, statusCode: 400, message: 'http error:' + httpStatusCode + errMsg, data: '', metaData: '', popup: false };
        });

    return json;

}

//以POST取得部分更新頁面內容的json回傳值
async function postFetchJson(actionName, postData, reqContentType) {
    /*
     * actionName:要呼叫的action name(或{controller}/{action})
     * postData:要上傳的參數或資料(必須轉換成x-www-form-urlencoded Content-Type)
     * reqContentType: request的Content-Type,例如：x-www-form-urlencoded或json
    */

    /*
     * 回傳json物件格式：
     *  {
     *      success:(bool:是否成功),
     *      statusCode: Http Status Code(server端自定義),
     *      message:結果訊息,
     *      data:內容/資料物件,
     *      popup:(bool:是否跳窗顯示訊息)
     *  }
     */

    if (reqContentType) {
        reqContentType = reqContentType.toLowerCase();
        if (reqContentType.indexOf('form') >= 0) reqContentType = 'application/x-www-form-urlencoded';
        if (reqContentType.indexOf('json') >= 0) reqContentType = 'application/json';
    }
    else {
        reqContentType = 'application/x-www-form-urlencoded';
    }

    let json = await fetch(actionName, {
        method: 'POST',
        cache: 'no-cache',
        headers: {
            'Content-Type': reqContentType
        },
        body: postData
    })

    .then((resp) => {
            if (resp.ok) {
                return resp.json();
            } else {
                return { success: false, statusCode: resp.status, message: `http error:${resp.status} ${resp.statusText}`, data: '', metaData: '', popup: false };
                //或是throw給後面catch()去處理
                //throw new Error('Http error:' + resposne.status);
            }
        })
        .catch((err) => {
            //一律回應http 400:bad request
            let errMsg = (err.message) ? err.message : 'bad request';
            return { success: false, statusCode: 400, message: 'http error:' + errMsg, data: '', metaData: '', popup: false };
        });

    return json;
}

//以fetch API 進行ajax呼叫，並取得json回傳值
async function FetchJson(url, method, reqBody, reqContentType = 'form', reqHeaders) {
    /*
     * url:要呼叫的url或action name(或{controller}/{action})
     * method: GET / POST
     * reqBody:要上傳的html form元素，或FormData物件，或js參數物件或json資料
     * reqContentType: request的Content-Type,例如：x-www-form-urlencoded(可簡化成: form), json, file, multipart
     * reqHeaders: 額外指定的request header(例如：Authorization)
    */

    /*
     * 回傳json物件格式：
     *  {
     *      success:(bool:是否成功),
     *      contentType:內容格式(json/html/text/blob)
     *      statusCode: Http Status Code(server端自定義),
     *      messages:結果訊息,
     *      data:內容/資料物件,
     *      metaData: data的描述資料(非必要)
     *      popup:(bool:是否跳窗顯示訊息)
     *  }
     */

    if (!method) {
        let alertMsg = 'FetchJson() failed: missing [method] argument.';
        console.log(alertMsg);
        alert(alertMsg);
        return;
    }

    method = method.toUpperCase();
    //content-type
    let reqContentType_orig = reqContentType;
    if (reqContentType_orig) {
        reqContentType_orig = reqContentType_orig.toLowerCase();
        if (reqContentType_orig.indexOf('form') >= 0) reqContentType = 'application/x-www-form-urlencoded';
        if (reqContentType_orig.indexOf('json') >= 0) reqContentType = 'application/json';
        if (reqContentType_orig.indexOf('file') >= 0) reqContentType = '';  //fetch API在上傳file時不可指定Content-Type header
        if (reqContentType_orig.indexOf('multipart') >= 0) reqContentType = '';  //fetch API在上傳multipart/form-data時不可指定Content-Type header
    }
    else {
        reqContentType = 'application/x-www-form-urlencoded';
    }

    //header

    let headers = {};

    if (reqContentType != '') {
        headers['Content-Type'] = reqContentType;
    }

    if (reqHeaders) {
        headers = Object.assign(header, reqHeaders);
    }

    //parameter
    let parameter = {
        method: method,
        cache: 'no-cache',
        headers: headers
    };

    //只有在POST時才可加入body參數，GET不可有body參數
    if (!reqBody) reqBody = {};
    if (method == "POST") {
        let body;
        if (reqContentType == 'application/x-www-form-urlencoded') {
            let formData = ConvertToFormData(reqBody);
            body = urlencodeFormData(formData);
        } else if (reqContentType == '') {
            //上傳內容中包含檔案，不可urlencode
            body = ConvertToFormData(reqBody);;
        } else {
            //json type
            body = ConvertToJsonString(reqBody);
        }
        parameter = Object.assign(parameter, { body: body });
    }

    //debugToken
    let debugToken = new URL(location.href)?.searchParams?.get('debugToken');
    if (debugToken && debugToken.length > 0) {
        if (url.includes('?')) {
            url += `&debugToken=${debugToken}`;
        } else {
            if (!url.endsWith('/')) {
                url += `/${debugToken}?debugToken=${debugToken}`;
            } else {
                url += `${debugToken}?debugToken=${debugToken}`;
            }
        }
    }

    let json = await fetch(url, parameter)
        .then( async (resp) => {
            let respContentType = resp.headers.get('content-type');
            let resolvedData;
            if (resp.ok) {
                //success result
                if (respContentType?.includes('application/json')) {
                    return resp.json();

                } else if (respContentType?.includes('text/html')) {
                    respContentType = 'html';
                    resolvedData = await resp.text().then(text => text);

                } else if (respContentType?.includes('text/plain')) {
                    respContentType = 'text';
                    resolvedData = await resp.text().then(text => text);

                } else {
                    respContentType = 'blob';
                    resolvedData = await resp.blob().then(blob => blob);
                }

                return {
                    success: true,
                    contentType: respContentType,
                    data: resolvedData,
                    statusCode: resp.status,
                    messages: '',
                    metaData: '',
                    popup: false
                };

            } else {
                //failed result
                if (respContentType?.includes('text/html')) {
                    respContentType = 'html';
                    resolvedData = await resp.text().then(html => html);

                } else if (respContentType?.includes('text/plain')) {
                    respContentType = 'text';
                    resolvedData = await resp.text().then(text => text);

                } else {
                    respContentType = 'text';
                    resolvedData = `http error:${resp.status} ${resp.statusText}`;

                }

                return {
                    success: false,
                    contentType: respContentType,
                    data: '',
                    statusCode: resp.status,
                    messages: `${resolvedData}`,
                    metaData: '',
                    popup: false
                };

                //或是throw給後面catch()去處理
                //throw new Error('Http error:' + resposne.status);
            }
        })
        .catch((err) => {

            try {
                console.log(`fetch error catched:${JSON.stringify(err)}`);
            } catch {
                console.log(`fetch error catched:${err}`);
            }
           
            let errMsg = (err.message) ? err.message : '400:bad request';
            return {
                success: false,
                contentType: 'text',
                data: '',
                statusCode: resp.status,
                messages: `http error catched:${errMsg}`,
                metaData: '',
                popup: false
            };

        });

    return json;
}

//以fetch API 進行ajax呼叫，並取得html回傳值
async function FetchHtml(url, method, reqBody, reqContentType = 'form', reqHeaders) {
    /*
     * url:要呼叫的url或action name(或{controller}/{action})
     * method: GET / POST
     * reqBody:要上傳的html form元素或js參數物件或json資料
     * reqContentType: request的Content-Type,例如：x-www-form-urlencoded(可簡化成: form), json, file, multipart
     * reqHeaders: 額外指定的request header(例如：Authorization)
    */

    /*
     * 回傳json物件格式：
     *  {
     *      success:(bool:是否成功),
     *      statusCode: Http Status Code(server端自定義),
     *      message:結果訊息,
     *      data:內容/資料物件,
     *      popup:(bool:是否跳窗顯示訊息)
     *  }
     */

    if (!method) {
        let alertMsg = 'FetchJson() failed: missing [method] argument.';
        console.log(alertMsg);
        alert(alertMsg);
        return;
    }

    method = method.toUpperCase();
    //content-type
    if (reqContentType) {
        reqContentType = reqContentType.toLowerCase();
        if (reqContentType.indexOf('form') >= 0) reqContentType = 'application/x-www-form-urlencoded';
        if (reqContentType.indexOf('json') >= 0) reqContentType = 'application/json';
        if (reqContentType.indexOf('file') >= 0) reqContentType = '';  //fetch API在上傳file時不可指定Content-Type header
        if (reqContentType.indexOf('multipart') >= 0) reqContentType = '';  //fetch API在上傳multipart/form-data時不可指定Content-Type header
    }
    else {
        reqContentType = 'application/x-www-form-urlencoded';
    }

    //header

    let headers = {};

    if (reqContentType != '') {
        headers['Content-Type'] = reqContentType;
    }

    if (reqHeaders) {
        headers = Object.assign(header, reqHeaders);
    }

    //parameter
    let parameter = {
        method: method,
        cache: 'no-cache',
        headers: headers
    };

    //只有在POST時才可加入body參數，GET不可有body參數
    if (!reqBody) reqBody = {};
    if (method == "POST") {
        let body;
        let formData = ConvertToFormData(reqBody);
        if (reqContentType == 'application/x-www-form-urlencoded') {
            body = urlencodeFormData(formData);
        } else if (reqContentType == '') {
            //上傳內容中包含檔案，不可urlencode
            body = formData;
        } else {
            //json type
            body = ConvertToJsonString(reqBody);
        }
        parameter = Object.assign(parameter, { body: body });
    }

    let html = await fetch(url, parameter)
        .then((resp) => {
            if (resp.ok) {
                return resp.text();
            } else {
                return `http error:${resp.status} ${resp.statusText}`;
                //或是throw給後面catch()去處理
                //throw new Error('Http error:' + resposne.status);
            }
        })
        .catch((err) => {
            //一律回應http 400:bad request
            let errMsg = (err.message) ? err.message : '400:bad request';
            return errMsg;
        });

    return html;
}


//將FormData中的key-value pair(entries)轉換並encode成x-www-form-urlencoded Content-Type 格式的字串
function urlencodeFormData(formData) {
    /*
     * formData:以new FormData(form); 所建立的formData物件
     */
    let s = '';
    function encode(str) { return encodeURIComponent(str).replace(/%20/g, '+'); }
    for (let pair of formData.entries()) {
        if (typeof pair[1] == 'string') {
            s += (s ? '&' : '') + encode(pair[0]) + '=' + encode(pair[1]);
        }
    }
    return s;
}

//將javascript參數物件或Html form元素轉換成換成javascript FormData物件
function ConvertToFormData(argsObject) {
    /*
     * argsObject: Html form元素，或者要轉成FormData的javascript參數物件，格式：{key1:value1,key2:value2}
     */

    if (!argsObject) {
        console.log('ConvertToFormData() failed. the argsObject is null');
        return new FormData();
    }
    let objTypeName = argsObject.constructor.name;
    if (objTypeName === 'FormData') return argsObject;
    if (objTypeName === 'HTMLFormElement') return new FormData(argsObject);
    if (argsObject instanceof $LighterObject) {
        if (argsObject.targets && argsObject.targets.length > 0) {
            let elm = argsObject.targets[0];
            if (elm.tagName == 'FORM') {
                return new FormData(elm);
            } else {
                console.log(`warning:ConvertToFormData(), argsObjects is $LighterObject but with target other than form element.`);
                return new FormData();
            }
        } else {
            console.log(`warning:ConvertToFormData(), argsObjects is $LighterObject but without target element.`);
            return new FormData();
        }
    }
    if (argsObject instanceof Object) {
        let formData = new FormData();
        try {
            for (const [key, value] of Object.entries(argsObject)) {
                //將參數項加入formData
                let valueNew = (value == null)?'':value; 
                formData.set(key, valueNew);
            }
        }
        catch (e) {
            console.log('ConvertToFormData() failed. ' + e);
        }
        return formData;
    } else {
        console.log('ConvertToFormData() failed. the argsObject is not an valid data type.');
    }

    return new FormData();
}

//將javascript參數物件或Html form元素轉換成換成json字串(***NOTE:當form的輸入項中有「多值(多選)欄位」時，此函式不適用***)
function ConvertToJsonString(argsObject) {
    if (!argsObject) return JSON.stringify({});
    let objTypeName = argsObject.constructor.name;
    if (objTypeName === 'FormData') return JSON.stringify(Object.fromEntries(argsObject));
    if (objTypeName === 'HTMLFormElement') return JSON.stringify(Object.fromEntries(new FormData(argsObject)));
    if (typeof argsObject == 'string') return argsObject;
    if (argsObject instanceof Object) return JSON.stringify(argsObject);
    return JSON.stringify({});
}

//合併二組FormData
function CombineFormData(formData1, formData2, overwrite = true) {
    /*
     * formData1:第1組FormData
     * formData2:第2組FormData
     * overwrite:是否覆寫相同key值的項目
     */
    if (!formData1 && !formData2) return new FormData();
    if (formData1 && !formData2) return formData1;
    if (formData2 && !formData1) return formData2;
    for (let p of formData2) {
        if (overwrite) {
            formData1.set(p[0],p[1]);
        } else {
            formData1.append(p[0], p[1]);
        }
    }
    return formData1;
}

//合併多組物件成為單一FormData
function PostData(...dataObjects) {
    return CombinePostData(...dataObjects);
}

//合併多組Post Data成為單一FormData
function CombinePostData(...dataObjects) {
    if (!dataObjects) return new FormData();
    if (dataObjects.length == 0) return new FormData();
    let combined = new FormData();
    for (const o of dataObjects) {
        combined = CombineFormData(combined, ConvertToFormData(o));
    }
    return combined;
}

//將傳入的javascript參數物件轉換成換成javascript FormData物件
//並encode成x-www-form-urlencoded Content-Type 格式的字串
//若指定額外的html form物件，會合併到FormData中
function createUrlEncodedFormData(argsObject, form) {
     /*
     * argsObject: 要轉成FormData的javascript參數物件，格式：{"key1":value1,"key2":value2}
     * form:要附加參數的表單(form)物件
     */

    let formData = ConvertToFormData(argsObject);
    let combined = CombineFormData(formData, new FormData(form));
    let encoded = urlencodeFormData(combined);
    return encoded;

}

//尋找上層元素
function FindParent(src, parentTagName, maxLevel) {
    /*
     * src:當下元素
     * parentTagName:要尋找的上層物件的tagName(例如：tr, div, span, ...),可指定多種tag用逗號分隔，若無指定則直接取上一層的任意元素
     * maxLevel:最多往上找幾層
     */
    if (!maxLevel) maxLevel = 1;
    if (!parentTagName) return src.parentElement;
    parentTagName = parentTagName.toLowerCase();
    let parent;
    for (let i = 0; i < maxLevel; i++) {
        parent = src.parentElement;
        if (parentTagName.indexOf(parent.tagName.toLowerCase()) >= 0) return parent;
        src = parent;
    }
    return parent;
}

//取得元素的值
function GetValue(element_or_id, defaultValue) {
    defaultValue = defaultValue || '';
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return defaultValue;
    }

    let nodeName = element.nodeName.toLowerCase();
    if (nodeName == 'select') {
        //datatype: string
        if (element.selectedIndex >= 0) {
            return element.value || element.options[element.selectedIndex].value;
        } else {
            return defaultValue;
        }
    }
    else if (nodeName == 'input') {
        let nodeType = element.getAttribute('type').toLowerCase();
        if (nodeType == 'checkbox') {
            //datatype: bool (true/false)
            return element.checked;
        } else if (nodeType == 'radio') {
            //radio is a special case that has no id, but with multiple element with the same name, so need to use $RadioGroup to get its value
            let radioGroupName = element.getAttribute('name');
            let rg = $RadioGroup(radioGroupName);
            if (rg) {
                return rg.checkedItem.value;
            } else {
                console.log(`warning: GetValue() failed. $RadioGroup of name=${radioGroupName} not exist`);
                return defaultValue;
            }

        } else {
            //datatype: string
            return element.value;
        }
    }
    else if (nodeName == 'textarea') {
        return element.value;
    }
    else if (nodeName == 'span' || nodeName == 'div' || nodeName == 'label') {
        return element.innerText;
    }
    else {
        return defaultValue ;
    }
}

//取得元素的文字(select/radio/等元素取其選項文字，而非value，或核取狀態；checkbox則需另外設計，因為不同情境下，適合取不同的結果)
function GetText(element_or_id, defaultValue) {
    defaultValue = defaultValue || '';
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return defaultValue ;
    }

    let nodeName = element.nodeName.toLowerCase();
    if (nodeName == 'select') {
        //datatype: string
        if (element.selectedIndex >= 0) {
            return element.options[element.selectedIndex].text || element.value;
        } else {
            return defaultValue ;
        }
    }
    else if (nodeName == 'input') {
        let nodeType = element.getAttribute('type').toLowerCase();
        if (nodeType == 'checkbox') {
            //datatype: bool (true/false)
            return element.checked;  //TO DO: 需更細部設計，以適合多數情況
        } else if (nodeType == 'radio') {
            //radio is a special case that has no id, but with multiple element with the same name, so need to use $RadioGroup to get its value
            let radioGroupName = element.getAttribute('name');
            let rg = $RadioGroup(radioGroupName);
            if (rg) {
                return rg.checkedItemLabelText;
            } else {
                console.log(`warning: GetValue() failed. $RadioGroup of name=${radioGroupName} not exist`);
                return defaultValue ;
            }

        } else {
            //datatype: string
            return element.value;
        }
    }
    else if (nodeName == 'textarea') {
        return element.value;
    }
    else if (nodeName == 'span' || nodeName == 'div' || nodeName == 'label') {
        return element.innerText;
    }
    else {
        let text = element.innerText;
        if (!text) text = defaultValue;
        return text ;
    }
}

//設定元素的文字(select/radio/等元素取其選項文字，而非value，或核取狀態；checkbox則需另外設計)
function SetText(element_or_id, text) {
    if (!text) text = '';
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`SetText() faild. [${element_or_id}] not found`);
        return;
    }

    let nodeName = element.nodeName.toLowerCase();
    if (nodeName == 'select') {
        //datatype: string
        if (element.selectedIndex >= 0) {
            element.options[element.selectedIndex].text = text;
        }
    }
    else if (nodeName == 'input') {
        console.log(`warning: SetText() aborted for input element. You may consider using SetValue().`);
    }
    else if (nodeName == 'textarea') {
        console.log(`warning: SetText() aborted for textarea element. You may consider using SetValue().`);
    }
    else if (nodeName == 'span' || nodeName == 'div' || nodeName == 'label') {
        element.innerText = text;
    }
    else {
        element.innerText = text;
    }
}

//取得元素對應的值和文字(回傳：{value:'ElementValue', text:'elementText'})
function GetValueText(element_or_id, withExtraData, defaultValue) {
    defaultValue = defaultValue || '';
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return { value: defaultValue , text: defaultValue, extraNames:'', extraValues:'' } ;
    }

    let nodeName = element.nodeName.toLowerCase();
    if (nodeName == 'select') {
        //datatype: string
        if (element.selectedIndex >= 0) {
            let option = element.options[element.selectedIndex];
            let v = element.value || option.value;
            let t = option.text;
            if (withExtraData) {
                let extraDataName = option.getAttribute('data-extra-name');
                let extraDataValue = option.getAttribute('data-extra-value');
                if (!extraDataName) extraDataName = '';
                if (!extraDataValue) extraDataValue = '';
                return { value: v, text: t, extraNames: extraDataName, extraValues: extraDataValue };
            } else {
                return { value: v, text: t, extraNames: '', extraValues: '' };
            }

        } else {
            return { value: defaultValue, text: defaultValue, extraNames: '', extraValues: '' };
        }
    }
    else if (nodeName == 'input') {
        let nodeType = element.getAttribute('type').toLowerCase();
        if (nodeType == 'checkbox') {
            //datatype: bool (true/false)
            return { value: element.checked, text: element.checked, extraNames: '', extraValues: '' } ;  //TO DO: 需更細部設計，以適合多數情況

        } else if (nodeType == 'radio') {
            //radio is a special case that has no id, but with multiple element with the same name, so need to use $RadioGroup to get its value
            let radioGroupName = element.getAttribute('name');
            let rg = $RadioGroup(radioGroupName);
            if (rg) {
                return { value: rg.checkedItem.value, text: rg.checkedItemLabelText };
            } else {
                console.log(`warning: GetValue() failed. $RadioGroup of name=${radioGroupName} not exist`);
                return { value: defaultValue, text: defaultValue, extraNames: '', extraValues: '' };
            }

        } else {
            //datatype: string
            return { value: element.value, text: element.value, extraNames: '', extraValues: '' };
        }
    }
    else if (nodeName == 'textarea') {
        return { value: element.value, text: element.value, extraNames: '', extraValues: '' };
    }
    else if (nodeName == 'span' || nodeName == 'div' || nodeName == 'label') {
        return { value: element.innerText, text: element.innerText, extraNames: '', extraValues: '' };
    }
    else {
        return { value: defaultValue, text: defaultValue, extraNames: '', extraValues: '' };
    }
}

//設定元素的值
function SetValue(element_or_id, value) {
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return;
    }
    if (!value & value != '0') value = '';
    let nodeName = element.nodeName.toLowerCase();
    if (nodeName == 'select') {
        //datatype: string
        //element.value = value;
        $SelectValue(element, value);
    }
    else if (nodeName == 'input') {
        let nodeType = element.getAttribute('type').toLowerCase();
        if (nodeType == 'checkbox') {
            //datatype: bool (true/false)
            element.checked = ConvertToBoolean(value);
        } else if (nodeType == 'datetime-local') {
            //format: yyyy-MM-ddTHH:mm
            let dt = new Date(value);
            let formatted = `${dt.getFullYear()}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}T${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0') }`;
            element.value = formatted;
        } else if (nodeType == 'date') {
            //format: yyyy-MM-dd
            let dt = new Date(value);
            let formatted = `${dt.getFullYear()}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}`;
            element.value = formatted;
        }
        else {
            //datatype: string
            element.value = value;
        }
    }
    else if (nodeName == 'textarea') {
        element.value = value;
    }
    else if (nodeName == 'span' || nodeName == 'div' || nodeName == 'label') {
        element.innerText = value;
    }
    else {
        console.log(`[${element_or_id}] value not set due no matching input type or tagName`);
    }
    return element;
}

//取得data attribute value
function GetData(element_or_id, dataAttributeName) {
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return '';
    }
    if (!dataAttributeName) {
        console.log(`[dataAttributeName] argument missing`);
        return '';
    }
    if (!dataAttributeName.startsWith('data-')) {
        dataAttributeName = `data-${dataAttributeName}`;
    }
    let dataValue = element.getAttribute(dataAttributeName);
    return dataValue;
}

//設定data attribute value
function SetData(element_or_id, dataAttributeName, value) {
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return '';
    }

    if (!dataAttributeName.startsWith('data-')) {
        dataAttributeName = `data-${dataAttributeName}`;
    }

    if (!value) value = '';

    element.setAttribute(dataAttributeName, value);
}

//設定元素的url
function SetUrl(element_or_id, url) {
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return;
    }
    if (element.hasAttribute('src')) {
        element.src = url;
    } else if (element.hasAttribute('href')) {
        element.href = url;
    } else {
        console.log(`SetUrl failed: ${element_or_id} has no url related attibute.`);
    }
}


/*Lighter Flag*/
function $ElementFlag(element_or_id, flagName) {
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return undefined;
    }
    if (!flagName) {
        console.log('missing [flagName] argument');
        return undefined;
    }
    this.element = element;
    this.flagName = flagName;

    return this;
}

Object.defineProperty($ElementFlag.prototype, 'value', {
    get() {
        return HasFlag(this.element, this.flagName);
    }
});

Object.defineProperty($ElementFlag.prototype, 'isOn', {
    get() {
        return HasFlag(this.element, this.flagName);
    }
});

$ElementFlag.prototype.bind = function (target, changeCallback) {
    let bindResult = true;
    if (target && target instanceof $LighterObject) {
        if (target.valid) {
            if (target.targets.length == 1) {
                bindResult = $SetElementBinding(this.element, this.flagName, target.targets[0], changeCallback);
            } else {
                bindResult = $SetElementBinding(this.element, this.flagName, target.targets, changeCallback);
            }
        } else {
            console.log(`warning: bind() aborted because the target is not a valid $LighterObject nor a HTMLElement`);
            bindResult = false;
        }
        bindResult = $SetElementBinding(this.element, this.flagName, target.targets, changeCallback);
    } else {
        bindResult = $SetElementBinding(this.element, this.flagName, target, changeCallback);
    }

    return bindResult;
}

$ElementFlag.prototype.on = function () {
    if (this.value == false) {
        SetFlagOn(this.element, this.flagName);
        $TriggerElementBinding(this.element,this.flagName,true);
    }
}

$ElementFlag.prototype.off = function () {
    if (this.value == true) {
        SetFlagOff(this.element, this.flagName);
        $TriggerElementBinding(this.element, this.flagName, false);
    }
}

$ElementFlag.prototype.toggle = function () {
    if (this.value) {
        SetFlagOff(this.element, this.flagName);
        $TriggerElementBinding(this.element, this.flagName, false);
    } else {
        SetFlagOn(this.element, this.flagName);
        $TriggerElementBinding(this.element, this.flagName, true);
    }

}


//針對指定的元素，設定旗標值
function SetFlag(element_or_id, flagName, flag_value) {
    if (!flagName) {
        console.log('missing [flagName] argument');
        return;
    } else {
        flagName = flagName.toLowerCase();
    }
    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return;
    }

    if (!flag_value) flag = "";
    element.setAttribute(`data-flag-${flagName}`, flag_value);
}

//針對指定的元素，取得旗標值
function GetFlag(element_or_id, flagName) {
    if (!flagName) {
        console.log('missing [flagName] argument');
        return '';
    } else {
        flagName = flagName.toLowerCase();
    }

    let element = $GetElement(element_or_id);
    if (!element) {
        console.log(`[${element_or_id}] not found`);
        return '';
    }

    let flag_value = element.getAttribute(`data-flag-${flagName}`);
    return flag_value;
}

//判斷某flag是否On
function HasFlag(element_or_id, flagName) {
    let flag = GetFlag(element_or_id, flagName);
    if (!flag) return false;
    flag = flag.toLowerCase();
    let on = (flag == 'true' || flag == '1' || flag == 'on' || flag == 'yes' || flag == 'y');
    return on;
}

//設定布林值flag=true
function SetFlagOn(element_or_id, flagName) {
    SetFlag(element_or_id, flagName, 1);
}

//設定布林值flag=false
function SetFlagOff(element_or_id, flagName) {
    SetFlag(element_or_id, flagName, 0);
}


/**Lighter Flag Binding**/
let $ElementBindingStore = new Map();
function $SetElementBinding(source, attributeName, target, changeCallback) {
    if (!source) {
        console.log(`warning: $SetElementBinding() aborted. The source argument is undefined.`);
        return false;
    }
    if (!attributeName) {
        console.log(`warning: $SetElementBinding() aborted. The attributeName argument is undefined.`);
        return false;
    }
    if (!target) {
        console.log(`warning: $SetElementBinding() aborted. The target argument is undefined.`);
        return false;
    }
    if (!changeCallback) {
        console.log(`warning: $SetElementBinding() aborted. The changeCallback argument is undefined.`);
        return false;
    }

    if (Array.isArray(source) && Array.isArray(target) && source.length > 1 && target.length > 1) {
        //M to M binding
        if (source.length != target.length) {
            console.log(`warning: $SetElementBinding() aborted because the source element counts is not equal to target element counts when doing M to M binding.`);
            return false;
        }
        let batchResult = true
        for (let i = 0; i < source.length; i++) {
            batchResult = batchResult && $SetElementBinding(source[i], attributeName, target[i], changeCallback);
        }
        return batchResult;
    }

    let attrMap = $ElementBindingStore.get(source);
    if (!attrMap) {
        attrMap = new Map();
        $ElementBindingStore.set(source,attrMap);
    }

    let bindingArr = attrMap.get(attributeName);
    if (!bindingArr) {
        bindingArr = [];
        attrMap.set(attributeName,bindingArr);
    }

    let targetArr;
    targetArr = (target.length) ? target : [target];

    for (let element of targetArr) {
        let bindingItem = bindingArr.find(x => x.target == element);
        if (!bindingItem) {
            bindingItem = { target: element, changeCallback: changeCallback }
            bindingArr.push(bindingItem);
        } else {
            let sourceExpr = $ElementExpression(source);
            let targetExpr = $ElementExpression(element);
            console.log(`warning: $ElementFlagBinding() aborted due to binding to attributeName=${attributeName} more than once from target=${targetExpr} to source=${sourceExpr}`);
        }
    }
    return true;
}

function $TriggerElementBinding(source, attributeName, value) {
    let attrMap = $ElementBindingStore.get(source);
    if (!attrMap) {
        console.log(`$TriggerElementBinding() aborted. No binding is set for source=${$ElementExpression(source)}`);
        return;
    }
    let bindingArr = attrMap.get(attributeName);
    if (!bindingArr || bindingArr.length == 0) {
        console.log(`$TriggerElementBinding() aborted. No binding is set for attributeName=${attributeName} of source=${$ElementExpression(source) }`);
        return;
    }

    for (let bindingItem of bindingArr) {
        try {
            let bindingArg = new $BindingTriggerArgument();
            bindingArg.value = value;
            bindingArg.source = source;
            bindingArg.target = bindingItem.target;
            bindingArg.attribute = attributeName;
            bindingItem.changeCallback(bindingArg);
        } catch (e) {
            console.log(`error: $TriggerElementBinding() failed for attributeName=${attributeName}, source=${$ElementExpression(source)}, message=${e}. You may check the changeCallback function of set binding.`);
        }
    }

}

function $BindingTriggerArgument() {
    this.value;
    this.source;
    this.attribute;
    this.target;
}

function $ElementExpression(element) {
    return `<${element.tagName} id=${element.id} name=${element.name} class=${element.classList} data-name=${element.getAttribute('data-name')} data-container-name=${element.getAttribute('data-container-name')} data-index=${element.getAttribute('data-index')}>`;
}

/**顯示控制**/
//顯示指定的元素
//若多個元素id則以逗號分隔
function Show(element_or_ids, displayMode) {
    /*
     * displayMode: 若有指定，會將其cssClass加上指定的displayMode,
     * 若無指定，則移除hide, hidden的cssClass後，維持其原本的display設定
     */
    if (element_or_ids) {
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let parent = x.parentElement;
                //有容器物件
                if (parent && parent.id && x.id && parent.id == x.id + '_container') {
                    parent.classList.remove('hide', 'hidden');
                    if (displayMode) {
                        parent.classList.add(`${displayMode}-important`);
                    }
                }
                //無容器物件
                else {
                    x.classList.remove('hide', 'hidden');
                    if (displayMode) {
                        x.classList.add(`${displayMode}-important`);
                    }
                }
            }
        });
    }
}

//隱藏指定的元素
//若多個元素id則以逗號分隔
function Hide(element_or_ids) {

    if (element_or_ids) {     
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let parent = x.parentElement;
                //有容器物件
                if (parent && parent.id && x.id && parent.id == x.id + '_container') {
                    parent.classList.remove('hide', 'show');
                    parent.classList.add('hide');
                }
                //無容器物件
                else {
                    x.classList.remove('hide', 'show');
                    x.classList.add('hide');
                }
            }
        });
    }
}

//切換顯示狀態
//若多個元素id則以逗號分隔
function Toggle(element_or_ids) {
    if (element_or_ids) {
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let parent = x.parentElement;
                //有容器物件
                if (parent && parent.id == x.id + '_container') {
                    parent.classList.toggle('hide');
                }
                //無容器物件
                else {
                    x.classList.toggle('hide');
                }
            }
        });
    }
}

//顯示指定的元素(若滿足src的值代入predicate中成立，若不成立則隱藏)
//若多個元素id則以逗號分隔
function ShowIf(src, predicate, show_element_or_ids) {
    /*
     * src:取值的來源物件/元素
     * predicate:判斷立成與否的條件式，將src物件的值代入後，回傳true/false
     */
    if (show_element_or_ids) {
        let value = GetInputElementValue(src);
        let showObjs = $GetElementArray(show_element_or_ids);
        showObjs.forEach(x => {
            if (x) {
                let parent = x.parentElement;
                //有容器物件
                if (parent && parent.id == x.id + '_container') {
                    parent.classList.remove('hide', 'show');
                    parent.classList.toggle('hide', !predicate(value));
                }
                //無容器物件
                else {
                    x.classList.remove('hide', 'show');
                    x.classList.toggle('hide', !predicate(value));
                }
            }
        });
    }
}

//隱藏指定的元素(若滿足src的值代入predicate中成立，若不成立則顯示)
//若多個元素id則以逗號分隔
function HideIf(src, predicate, hide_element_or_ids) {
    /*
     * src:取值的來源物件/元素
     * predicate:判斷立成與否的條件式，將src物件的值代入後，回傳true/false
     */
    if (hide_element_or_ids) {
        let value = GetInputElementValue(src);
        let hideObjs = $GetElementArray(hide_element_or_ids);
        hideObjs.forEach(x => {
            if (x) {
                let parent = x.parentElement;
                //有容器物件
                if (parent && parent.id == x.id + '_container') {
                    parent.classList.remove('hide', 'show');
                    parent.classList.toggle('hide', predicate(value));
                }
                //無容器物件
                else {
                    x.classList.remove('hide', 'show');
                    x.classList.toggle('hide', predicate(value));
                }
            }
        });
    }
}

//切換顯示狀態(依src的值代入predicate中成立與否，切換showIds和hideIds二組元素的顯示/隱藏狀態)
//若多個元素id則以逗號分隔
function ToggleIf(src, predicate, show_element_or_ids, hide_element_or_ids) {
    /*
     * src:取值的來源物件/元素
     * predicate:判斷立成與否的條件式，將src物件的值代入後，回傳true/false
     * showIds:條件成立時，要顯示的元素id(若有多個以逗號分隔)
     * hideIds:條件成立時，要隱藏的元素id(若有多個以逗號分隔)
     */

    ShowIf(src, predicate, show_element_or_ids);
    HideIf(src, predicate, hide_element_or_ids);

}


//清除指定的元素的內容
//若多個元素id則以逗號分隔
function Clear(element_or_ids, textOnly) {
    /*
     *textOnly: 是:只清除文字內容，否:清除Html內容
     */
    if (element_or_ids) {
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let nodeName = x.nodeName.toLowerCase();
                if (nodeName == 'input') {
                    let type = x.getAttribute('type');
                    switch (type) {
                        case 'textbox':
                        case 'password':
                        case 'hidden':
                        case 'email':
                        case 'date':
                        case 'datetime':
                        case 'time':
                        case 'file':
                        case 'text':
                        case 'search':
                            x.setAttribute('value', '');
                            x.value = '';
                            break;
                        case 'checkbox':
                        case 'radio':
                            x.setAttribute('checked', '');
                            x.checked = false;
                            break;
                    }
                }
                else {
                    if (textOnly) {
                        x.innerText = '';
                    } else {
                        x.innerHTML = '';
                    }
                }
            }
        });
    }
}


//重設指定的元素的內容(與Clear區別在form, select元素處理方式)
//若多個元素id則以逗號分隔
function Reset(element_or_ids) {
    if (element_or_ids) {
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let nodeName = x.nodeName.toLowerCase();
                if (nodeName == 'form') {
                    x.reset();
                }
                else if (nodeName == 'select') {
                    x.selectedIndex = 0;
                }
                else {
                    Clear(elementIds);
                }
            }
        });
    }
}

//重設指定的元素的內容(與Clear區別在form, select元素處理方式)(若滿足src的值代入predicate中成立，則重設)
//若多個元素id則以逗號分隔
function ResetIf(src, predicate, element_or_ids) {
    /*
     * src:取值的來源物件/元素
     * predicate:判斷立成與否的條件式，將src物件的值代入後，回傳true/false
     */

    let value = GetInputElementValue(src);

    if (predicate(value) == false) return;

    if (element_or_ids) {
        let objs = $GetElementArray(element_or_ids);
        objs.forEach(x => {
            if (x) {
                let nodeName = x.nodeName.toLowerCase();
                if (nodeName == 'form') {
                    x.reset();
                }
                else if (nodeName == 'select') {
                    x.selectedIndex = -1;
                }
                else {
                    Clear(elementIds);
                }
            }
        });
    }
}

/***[訊息] 函式***/

//顯示訊息
function ShowMessage(msgList,msgContainerId) {
    /*
     * msgList:訊息內容清單，json格式如下:
     *         [
     *          {
     *              "msgType": "訊息種類(info/warning/error/confirm/debug)",
     *              "text": "訊息文字(可html內容)",
     *              "caption": "訊息標題"
     *          },
     *          ...
     *         ]
     * msgContainerId:要顯示訊息的容器元素id (預設值：msg_container_top)
     * NOTE: text變數稱，也可使用message變數名稱
     *       caption變數名稱，也可以使用title變數名稱
     */

    if (!msgList) {
        console.log('msgList is empty');
        return;
    }

    //轉成array object
    let msgArr = ConvertToJSObject(msgList);
    if (msgArr) {
        //若非陣列，包成陣列
        if (!Array.isArray(msgArr)) {
            msgArr = [msgArr];
        }
    } else {
        //傳入的是純字串, 包成陣列
        msgArr = [{type:'error',text:msgList}];
    }

    if (!msgContainerId) msgContainerId = '#msg_container_top';
    if (!msgContainerId.startsWith('#')) msgContainerId = '#' + msgContainerId
    let container = document.querySelector(msgContainerId);
    if (container) {
        container.innerHTML = '';
        container.classList.remove('msg-error', 'msg-info', 'msg-warning', 'hide', 'hidden');
        //let ul = document.createElement('ul');
        //ul.classList.add('msg-list');
        //container.appendChild(ul);
        msgArr.forEach(msg => {
            let messageText = '';
            if (msg.text) messageText += msg.text;
            if (msg.message) messageText += msg.message;
            let item = document.createElement('div');
            item.classList.add('msg-item', 'msg-' + msg.msgType.toLowerCase());
            item.innerHTML = messageText;
            container.appendChild(item);
        });
        if (container.style && container.style.display && container.style.display == 'none') {
            container.style.display = 'block';
        }
        container.scrollIntoView();
    } else {
        console.log(msgArr);
    }
}

//清除訊息
function ClearMessage(msgContainerId) {
    /*
     * containerId:顯示訊息的容器元素id
     */
    if (!msgContainerId) msgContainerId = '#msg_container_top';
    if (!msgContainerId.startsWith('#')) msgContainerId = '#' + msgContainerId
    let container = document.querySelector(msgContainerId);
    if (container) {
        container.innerHTML = '';
        container.classList.add('hide');
    }
}

//顯示FetchJson失敗訊息
function ShowFetchJsonFailMessage(form, json) {
    //資料檢核錯誤訊息
    if (json.modelErrors && json.modelErrors.length > 0) {
        SetValidationHint(form, json.modelErrors);
    }
    //其他提示訊息
    if (json.messages && json.messages.length > 0) {
        if (json.isPopup) {
            for (m of json.messages) {
                let caption = m.caption || '系統提示';
                let msg = {
                    msgType: m.msgType.toLowerCase(),
                    title: caption,
                    message: `${m.text}`
                }
                $PopMessage(msg);
            }
  
        } else {
            for (m of json.messages) {
                if (m.isPopup) {
                    let caption = m.caption || '系統提示';
                    let msg = {
                        msgType: m.msgType.toLowerCase(),
                        title: caption,
                        message: `${m.text}`
                    }
                    $PopMessage(msg);
                } else {
                    ShowMessage(m);
                }
            }
        }
    }
}

//取得元素的畫面位置
function $Position(element_or_selector) {
    let defaultPosition = { top: 0, left: 0, width: 0, height: 0, bottom: 0, right: 0 };
    let elm = $GetElement(element_or_selector);
    if (elm) {
        if (elm.getBoundingClientRect) {
            return elm.getBoundingClientRect();
        }
    } else {
        return defaultPosition;
    }

    return defaultPosition;
}

//取得元素的畫面位置、尺寸
function $ElementRect(element_or_selector) {
    let pos = { top: 0, left: 0, width: 0, height: 0, bottom: 0, right: 0, pageTop: 0, pageLeft: 0 };
    let elm = $GetElement(element_or_selector);
    if (elm) {
        if (elm.getBoundingClientRect) {
            let rect = elm.getBoundingClientRect();
            pos.top = rect.top;
            pos.left = rect.left;
            pos.width = rect.width;
            pos.height = rect.height;
            pos.bottom = rect.bottom;
        }
        pos.pageTop = pos.top + Math.max(window.pageYOffset, document.documentElement.scrollTop, document.body.scrollTop);
        pos.pageLeft = pos.left + Math.max(window.pageXOffset, document.documentElement.scrollLeft, document.body.scrollLeft);
    } else {
        return pos;
    }

    return pos;
}


//延遲(毫秒數)
function Delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

//顯示轉轉轉(於畫面中央)
function ShowSpinnerScreenCenter(scale, text, maxTime) {
    /*
    * scale: 1~5 (size由小至大)
    * text:文字內容
    * maxTime:最大秒數，超過此秒數spinner會自動關閉(預設值10)
    */
    let maxWidth = 240;  //最大寬度150px
    let maxScale = 5;
    let maxTimeout = 10; //最大timeout:10秒
    if (!scale) scale = maxScale;
    let realWidth = parseInt((scale / maxScale) * maxWidth, 10);
    let padding = parseInt(realWidth * 0.1, 10);
    let spinner = document.createElement("div");
    let spinnerWrapper = document.createElement("div");
    let contentContainer = document.createElement("div");
    let textDiv = document.createElement("div");
    let percentDiv = document.createElement("div");
    let progressBar = document.createElement("progress");

    spinner.classList.add("spinner");
    spinner.style.width = realWidth + 'px';
    spinner.style.height = realWidth + 'px';
    spinner.style.padding = padding + 'px';

    percentDiv.classList.add("spinner-percent");

    progressBar.classList.add("hide");
    progressBar.max = 100;
    progressBar.value = 0;

    textDiv.classList.add("spinner-text");
    if (text) textDiv.innerText = text;

    contentContainer.classList.add("center-fixed");
    contentContainer.classList.add("spinner-content");
    contentContainer.appendChild(percentDiv);
    contentContainer.appendChild(progressBar);
    contentContainer.appendChild(textDiv);

    spinnerWrapper.style.width = realWidth + 'px';
    spinnerWrapper.style.height = realWidth + 'px';
    spinnerWrapper.classList.add("center-fixed");
    spinnerWrapper.appendChild(spinner);
    spinnerWrapper.appendChild(contentContainer);

    //spinner.class = 'screen-center-fixed';

    //spinner.setAttribute('class','screen-center-fixed');

    //let containerStyle = GetActualStyle(container);
    //if (containerStyle.getPropertyValue('display').indexOf('flex') >= 0) {
    //    spinner.style.justifySelf = 'center';
    //} else {
    //    let position = GetAlignCenterPosition(container, spinner);
    //    spinner.style.top = position.top + 'px';
    //    spinner.style.left = position.left + 'px';
    //}

    document.body.appendChild(spinnerWrapper);

    if (!maxTime) maxTime = maxTimeout;
    window.setTimeout(() => {
        StopSpinner(spinnerWrapper);
    }, maxTime * 1000);

    return spinnerWrapper;
}

//顯示轉轉轉(於容器元素中)
function ShowSpinner(container, scale, maxTime) {
    /*
    container:容器元素
    scale: 1~5 (size由小至大)
    maxTime:最大秒數，超過此秒數spinner會自動關閉
    */
    let maxWidth = 150;  //最大寬度150px
    let maxScale = 5;
    let maxTimeout = 10; //最大timeout:10秒
    if (!scale) scale = maxScale;
    let realWidth = parseInt((scale / maxScale) * maxWidth, 10);
    let padding = parseInt(realWidth * 0.1, 10);
    let spinner = document.createElement("div");
    spinner.classList.add("spinner");
    spinner.style.width = realWidth + 'px';
    spinner.style.height = realWidth + 'px';
    spinner.style.padding = padding + 'px';

    let containerStyle = GetActualStyle(container);
    if (containerStyle.getPropertyValue('display').indexOf('flex') >= 0) {
        spinner.style.justifySelf = 'center';
    } else {
        let position = GetAlignCenterPosition(container, spinner);
        spinner.style.top = position.top + 'px';
        spinner.style.left = position.left + 'px';
    }

    container.appendChild(spinner);

    if (!maxTime) maxTime = maxTimeout;
    window.setTimeout(() => {
        StopSpinner(spinner);
    }, maxTime * 1000);

    return spinner;
}

//停止轉轉轉
function StopSpinner(spinner, delayTime, callback) {
    /*
     * spinner: 要停止的spinner物件
     * delayTime:要延遲的秒數
     * callback: 停止spinner後要接續的動作(若無則略)
     */

    if (!delayTime) delayTime = 0;
    delayTime = parseInt(delayTime) * 1000;

    Delay(delayTime).then(() => {
        if (spinner) {
            try {
                spinner.remove();
            } catch (err) {
                console.log('StopSpinner() failed:' + err);
            }
        }
        if (callback) callback();
    });

}

//設定spinner的進度
function SetSpinnerProgress(spinner, percent) {
    /*
     * spinner: spinner物件
     * percent: 介於 0 ~ 100之數字
     */
    if (!spinner) return;
    if (!percent) percent = 0;
    percent = Math.floor(percent);

    let progressBar = spinner.querySelector(".spinner-progress");
    let percentDiv = spinner.querySelector(".spinner-percent");
    if (progressBar) {
        progressBar.classList.remove('hide');
        progressBar.classList.remove('hidden');
        progressBar.value = percent;
    }
    if (percentDiv) percentDiv.innerText = percent + '%';
}

//取得實際style物件
function GetActualStyle(element) {
    let style;
    if (typeof window.getComputedStyle != "undefined") {
        style = window.getComputedStyle(element, null);
    } else if (element.currentStyle != "undefined") {
        style = element.currentStyle;
    }
    return style;
}

//取得置中對齊位置left,top
function GetAlignCenterPosition(container, element) {
    let container_rect = container.getBoundingClientRect();
    let container_t = container_rect.top + Math.max(window.pageYOffset, document.documentElement.scrollTop, document.body.scrollTop);
    let container_l = container_rect.left + Math.max(window.pageXOffset, document.documentElement.scrollLeft, document.body.scrollLeft);
    
    let container_w = container.clientWidth || container.style.width;
    let container_h = container.clientHeight || container.style.height;
    let element_w = (element.offsetWidth || element.clientWidth || element.style.width).replace(/px$/, '');
    let element_h = (element.offsetHeight || element.clientHeight || element.style.height).replace(/px$/, '');
    //console.log('container id=' + container.id);
    //console.log('container_w=' + container_w);
    //console.log('container_h=' + container_h);
    //console.log('element_w=' + element_w);
    //console.log('element_h=' + element_h);
    let top = container_t + parseInt((container_h - element_h) / 2, 10);
    let left = container_l + parseInt((container_w - element_w) / 2, 10);
    return { top: top, left: left };
}

/*State Machine*/
//State machine transition rule
function $StateTransitionRule(trigger, fromState, toState, region) {
    /*
     * trigger:狀態變更觸發動作
     * fromState:從此狀態
     * toState:轉換至此狀態
     * region: Model歸屬區域(預設：none)
     */
    this.trigger = trigger;
    this.fromState = fromState;
    this.toState = toState;
    if (!region) region = 'none';
    this.region = region;
}

// general state machine
function $StateMachine(state, region) {
    if (!state) state = 'inital';
    if (!region) region = 'none';
    this.state = state;
    this.region = region;
    this.transitionRules = [];  //transition rules collection
}

//add transition rule
$StateMachine.prototype.addRule = function (trigger, fromState, toState, region) {
    if (!region) region = 'none';
    if (this.transitionRules.length > 0) {
        //check rule conflick
        for (let rule of this.transitionRules) {
            if (rule.trigger == trigger && rule.fromState == fromState && rule.region == region && rule.toState != toState) {
                let message = `error: addRule() for an state machine failed. trigger=${trigger}, fromState=${fromState}, toState=${toState}, region=${region} conflick with existing rule of toState=${rule.toState}.`;
                console.log(message);
                let msg = { msgType: 'error', caption: '系統提示', message: message };
                $PopMessage(msg);
                return;
            }
        }
    }

    this.transitionRules.push(new $StateTransitionRule(trigger, fromState, toState, region));
}

//change state according to the transition rules of a state machine
$StateMachine.prototype.changeState = function (trigger) {

    let fromState = this.state;
    let region = this.region;

    let ruleFound = false;
    if (this.transitionRules.length > 0) {
        for (let rule of this.transitionRules) {
            if (rule.trigger == trigger && (rule.fromState == fromState || rule.fromState == 'any') && rule.region == region) {
                ruleFound = true;
                this.state = rule.toState;
            }
        }
    } else {
        console.log(`warning: changeState(), transitionRules.length = 0, current state(${this.state}) is kept unchanged as fromState=${fromState}.`);
    }

    if (!ruleFound && this.transitionRules.length > 0) {
        console.log(`warning: changeState(), transitionRules not found for trigger=${trigger}, fromState=${fromState}, region=${region}, current state(${this.state}) is kept unchanged`);
    }

    console.log(`changeState() trigger=${trigger} fromState=${fromState} toState=${this.state}`);
}

/*Table相關*/
//Row State 列舉
function $ModelStateEnumObject() {
    this.initial = 'initial'; //初始狀態(無狀態)
    this.any = 'any'; //任何狀態
    this.original = 'original'; //原始狀態(未變更的狀態)
    this.new = 'new';   //新增中(只會又一筆)
    this.added = 'added';   //新增
    this.updating = 'updating'; //編輯中
    this.updated = 'updated'; //編輯
    this.deleted = 'deleted'; //刪除
    this.abandoned = 'abandoned'; //捨棄
}

let $ModelState = new $ModelStateEnumObject();

//Row State Change Trigger列舉
function $ModelTriggerEnumObject() {
    this.add = 'add';
    this.edit = 'edit';
    this.accept = 'accept';
    this.delete = 'delete';
    this.cancel = 'cancel';
    this.save = 'save';
}

//data model state machine transition rules
function $ModelStateMachineRules() {
    let st = new $ModelStateEnumObject();
    let tg = new $ModelTriggerEnumObject();

    let sm = new $StateMachine();
    sm.addRule(tg.add, st.initial, st.new, 'client');
    sm.addRule(tg.accept, st.new, st.added, 'client');
    sm.addRule(tg.edit, st.new, st.updating, 'client');
    sm.addRule(tg.edit, st.added, st.updating, 'client');
    sm.addRule(tg.accept, st.updating, st.added, 'client');
    sm.addRule(tg.delete, st.any, st.abandoned, 'client');

    sm.addRule(tg.edit, st.original, st.updating);
    sm.addRule(tg.accept, st.updating, st.updated);
    sm.addRule(tg.edit, st.updated, st.updating);
    sm.addRule(tg.delete, st.any, st.deleted);

    return sm.transitionRules;
}

//帶狀態的data model, 對應table row的資料源
function $StateModel(model, stateMachine, bindingElement, isDirty, iscomplete) {
    this.model = model;
    this.stateMachine = stateMachine; 
    this.bindingElement = bindingElement;
    if (isDirty === undefined) isDirty = false;
    if (iscomplete == undefined) iscomplete = false;
    this.isDirty = isDirty; //欄位是否有變更
    this.iscomplete = iscomplete; //欄位是否已填寫完畢
}

//model的目前狀態
Object.defineProperty($StateModel.prototype, 'state', {
    get() {
        if (!this.stateMachine) {
            console.log(`get() failed. The current $StateModel has no stateMachine attached.`);
            return undefined;
        }
        return this.stateMachine.state;
    },
    set(state) {
        if (!this.stateMachine) {
            console.log(`set() failed. The current $StateModel has no stateMachine attached.`);
            return ;
        }
        this.stateMachine.state = state;
    }
});

//model歸屬的區域
Object.defineProperty($StateModel.prototype, 'region', {
    get() {
        if (!this.stateMachine) {
            console.log(`get() failed. The current $StateModel has no stateMachine attached.`);
            return undefined;
        }
        return this.stateMachine.region;
    },
    set(region) {
        if (!this.stateMachine) {
            console.log(`set() failed. The current $StateModel has no stateMachine attached.`);
            return;
        }
        this.stateMachine.region = region;
    }
});

//欄位控制器
function $FieldController(cellIndex, fieldName, fieldText, required, unique) {
    if (!cellIndex) cellIndex = 0;
    this.cellIndex = cellIndex;
    this.fieldName = fieldName;
    this.fieldText = fieldText;
    this.required = required;
    this.unique = unique;
    this.dataType;
    this.format;
    this.max;
    this.min;
    this.classList;
    this.validate;
}

//Lighter Table Object
function $TableObject(table_or_id, rowSelectEventHandler, rowCommandEventHandler, canEdit) {
    let table = $GetElement(table_or_id);
    if (!table) { console.log(`$TableObject() failed. the given ${table_or_id} is not a valid table argument`); }
    this.table = table;
    this.tableContainer = undefined;
    this.noData = undefined;
    this.rowStateTransitionRules = $ModelStateMachineRules();
    this.stateDataList = new Map(); //帶狀態的資料List(key值: temp_id)
    this.fieldControllers = new Map(); //欄位控制器(key值：field_name)
    this.uniqueFieldGroups = [];      //唯一性的欄位群組(以逗號分隔欄位名稱構成一組)
    this.highlightRows = []; //目前有醒目提示的列
    this.existingRows = [];  //唯一性檢核時，既存欄位所在的列
    this.rowTemplate = undefined;
    this.rwdCutLevel = 0;
    this.currentRow = undefined;
    this.focusElement = undefined;
    this.canAdd = false;
    this.canEdit = canEdit;
    this.canDelete = false;
    this.submitActionName = undefined;

    this.dataScopeContainer;
    this.dataScopeName='';

    if (this.canEdit) {
        this.dataScopeContainer = table.closest('[data-scope]');
        if (this.dataScopeContainer) {
            this.dataScopeName = this.dataScopeContainer.getAttribute('data-scope') || '';
        } else {
            console.log(`warning: container element with [data-scope] attribute is not found and may lead to functional button binding failure.`);
        }
    }

    this.isButtonBind = false; //按鈕是否已經綁定過(防止重複綁定)
    this.addButton;
    this.editButton;
    this.saveButton;
    this.cancelButton;
    this.deleteButton;
    this.searchButton;

    this.tableContainer = table.parentElement;
    this.noData = this.tableContainer.querySelector('[data-no-data]');
    this.rwdCutLevel = table.getAttribute('data-rwd-cutLevel');
    this.rowTabIndex = table.getAttribute('data-row-tabindex');

    this.id = table.id;
    this.rowSelectEventHandler = rowSelectEventHandler;

    this.rowCommandEventHandler = rowCommandEventHandler;
    //if (this.rowSelectEventHandler) {
    //    RegisterTableRowSelectedEvent(this.id, this.rowSelectEventHandler);
    //}
    this.setRowSelectHandler();
    this.rowCommandHandlerRegistered = false;  //列命令事件處理是否已註冊(防止重複)
    if (!this.rowCommandHandlerRegistered && this.rowCommandEventHandler) {
        this.setRowCommandHandler();
        //RegisterTableRowCommandEvent(this.table, this.rowCommandEventHandler);
    }
}

//是否合格的有效物件
$TableObject.prototype.valid = function(){
    return (this.table) ? true : false;
}

//table innerHTML
Object.defineProperty($TableObject.prototype, 'innerHTML', {
    get() {
        return this.table.innerHTML;
    },
    set(html) {
        this.table.innerHTML = html;
    }
});

//table body
Object.defineProperty($TableObject.prototype, 'tbody', {
    get() {
        if (this.table.tBodies.length = 0) {
            this.table.appendChild(document.createElement('tbody'));
        }
        return this.table.tBodies[0];
    }
});

//table rows array (only for tbody)
Object.defineProperty($TableObject.prototype, 'rows', {
    get() {
        let trs = this.tbody.querySelectorAll('tr');
        if (!trs || trs.length == 0) {
            return [];
        } else {
            return [...trs];
        }
    }
});

//table has data row
Object.defineProperty($TableObject.prototype, 'hasDataRow', {
    get() {
        let row = this.tbody.querySelector('tr');
        return (row) ? true : false;
    }
});

//change list
Object.defineProperty($TableObject.prototype, 'changeList', {
    get() {
        if (!this.stateDataList || this.stateDataList.size == 0) return [];
        let dataListArr = [...this.stateDataList.values()];
        let changed = dataListArr.filter(x => x.state == 'added' || x.state == 'updated' || x.state == 'deleted');
        if (!changed || changed.length == 0) return [];
        return changed.map(x => ({ State: x.state, Model: x.model }));
    }
});

//thead
Object.defineProperty($TableObject.prototype, 'thead', {
    get() {
        return this.table.tHead;
    }
});

//rwdState
Object.defineProperty($TableObject.prototype, 'rwdState', {
    get() {
        return this.table.getAttribute('data-rwd-state');
    },
    set(state) {
        this.table.setAttribute('data-rwd-state', state);
    }
});	

//current stateModel
Object.defineProperty($TableObject.prototype, 'currentStateModel', {
    get() {
        if (!this.currentRow) return null;
        let temp_id = this.currentRow.getAttribute('data-temp-id');
        if (temp_id) {
            return this.stateDataList.get(temp_id);
        } else {
            console.log(`get currentModel failed. [data-temp-id] missing for currentRow`);
            return null;
        }
    }
});

//current row data model
Object.defineProperty($TableObject.prototype, 'currentRowModel', {
    get() {
        let stateModel = this.currentStateModel;
        if (stateModel) {
            return stateModel.model;
        } else {
            if (this.stateDataList.size > 0) {
                console.log(`warning: currentRowModel not exist.`);
            }
            return null;
        }
    }
});

//model list
Object.defineProperty($TableObject.prototype, 'modelList', {
    get() {
        if (this.stateDataList.size == 0) return [];
        return [...this.stateDataList.values()].map(x => x.model);
    }
});

//加入欄位組合唯一性群組(以逗號分隔欄位名稱)
$TableObject.prototype.addUniqueFieldsGroup = function (fields) {
    this.uniqueFieldGroups.push(fields);
}

//設定列選取事件處理
$TableObject.prototype.setRowSelectHandler = function (trs) {
    /*
     * trs:要設定的列，指定單列或多列(陣列)的<tr>，若未給入，則納入全部列
     * 若canEdit==true，切換列成編輯模式
     * 若canEdit==false，觸發自定義的rowSelectEventHandler
     */
    let thisTable = this;
    if (this.canEdit) {
        for (let tr of this.rows) {
            tr.addEventListener('focus', function (e) {
                thisTable.changeRowMode.apply(thisTable,[tr]);
            });
        }
    } else {

        if (this.rowSelectEventHandler) {
            let rows;
            if (!trs) {
                rows = this.rows; //全部列
            } else {
                rows = Array.isArray(trs) ? trs : [trs];
            }
            for (const tr of rows) {
                //防止重複註冊事件處理
                let hasHandler = tr.getAttribute('data-has-row-select-handler');
                if (!hasHandler) {
                    tr.addEventListener('focus', function (e) {
                        thisTable.currentRow = tr;
                        tr.blur();

                        let args = new TableSelectedRow();
                        args.table_id = thisTable.id;
                        args.selected_tr = tr;
                        args.dataKeyStr = GetRowDataKey(tr);            //只有鍵值欄位的「值」多組以逗號分隔
                        args.dataKeyObj = GetRowDataKeyObject(tr);      //有鍵值欄位的「欄名」和「值」的物件形式
                        args.row_data_key = args.dataKeyStr;            //for舊版相容
                        args.dataModel = thisTable.currentRowModel;     //表格列所對應的data model
                        args.command_name = 'select';
                        //保存選取列
                        SetTableSelectedRow(args);

                        let dataKeyStr = GetRowDataKey(tr); //只有鍵值欄位的「值」多組以逗號分隔
                        let dataKeyObj = GetRowDataKeyObject(tr);//有鍵值欄位的「欄名」和「值」的物件形式

                        thisTable.rowSelectEventHandler.apply(null, [dataKeyStr, dataKeyObj]);
                    });

                    tr.setAttribute('data-has-row-select-handler','true');
                }
            }
        } else {
            console.log(`waring: setRowSelectHandler(). rowSelectEventHandler for table(id=${this.id}) is not specified.`);
        }

    }

    //for (const tr of this.rows) {
    //    tr.addEventListener('focus', function (e) {
    //        tr.blur();
    //        let row_data_key = GetRowDataKey(tr);
    //        //let model = new TableSelectedRow();
    //        //model.table_id = tableId;
    //        //model.selected_tr = tr;
    //        //model.row_data_key = row_data_key;
    //        //model.command_name = 'select';

    //        ////保存選取列
    //        //SetTableSelectedRow(model);

    //        rowSelectHandler(row_data_key);
    //        //e.stopPropagation(); //停止event bubbling
    //    });
    //}
}

//設定列命令處理
$TableObject.prototype.setRowCommandHandler = function (rowCommandHandler) {
    let thisTable = this;
    if (!thisTable.rowCommandEventHandler && rowCommandHandler) {
        thisTable.rowCommandEventHandler = rowCommandHandler;
    }

    if (!thisTable.rowCommandEventHandler) {
        console.log(`setRowCommandHandler() aborted because rowCommandEventHandler is not set.`);
        return;
    }

    if (thisTable.rowCommandHandlerRegistered) {
        console.log(`setRowCommandHandler() aborted because rowCommandEventHandler has already been registered.`);
        return;
    }

    thisTable.rowCommandHandlerRegistered = true;  //防止重複

    thisTable.table.addEventListener('click', function (e) {

        //判斷是否命令按鈕
        let src = e.target;
        if (src.matches("[data-command]")) {
            //找出所在的列
            let tr = FindParent(src, 'tr', 4);

            thisTable.currentRow = tr;

            let args = new TableSelectedRow();
            args.table_id = thisTable.id;
            args.selected_tr = tr;
            args.dataKeyStr = GetRowDataKey(tr);            //只有鍵值欄位的「值」多組以逗號分隔
            args.dataKeyObj = GetRowDataKeyObject(tr);      //有鍵值欄位的「欄名」和「值」的物件形式
            args.row_data_key = args.dataKeyStr;            //for舊版相容
            args.dataModel = thisTable.currentRowModel;     //表格列所對應的data model
            let command_name = src.getAttribute('data-command');
            args.command_name = command_name ? command_name : '';
            //保存選取列
            SetTableSelectedRow(args);
            //觸發handler
            //rowCommandHandler(model);
            thisTable.rowCommandEventHandler.apply(null, [args]);
        }
        e.stopPropagation(); //停止event bubbling
    });

}

//切換列成編輯模式
$TableObject.prototype.changeRowMode = function (tr) {

    tr.blur();

    if (!this.rowTemplate) {
        let msg = { msgType: 'error', caption: '系統提示', message: '找不到編輯所需的template, 請於View頁面中設置一段template,並在tempalte容器元素中設定data-template-for="tbRecuritRound"屬性。' };
        $PopMessage(msg);
        return;
    }

    //只適用可編輯canEdit=true的情況
    if (this.canEdit) {
        let rowMode = tr.getAttribute('data-row-mode');
        if (rowMode == 'edit') return;  //已經是編輯模式

        let temp_id = tr.getAttribute('data-temp-id');
        if (!temp_id) {
            console.log(`changeRowMode() failed. [data-temp-id] is undefined for tr.`);
            return;
        }

        tr.setAttribute('data-row-mode', 'edit');

        tr.innerHTML = this.rowTemplate.innerHTML;
        let stateModel = this.stateDataList.get(temp_id);
        if (stateModel) {
            $ApplyModel(stateModel.model, tr); //套用欄位值
        } else {
            console.log(`changeRowMode() temp_id=${temp_id} not exist in stateDataList`);
        }

        //設定欄位元素事件處理
        let inputs = tr.querySelectorAll('input, select, textarea');
        $EventHandler(inputs, 'change', (e) => {
            console.log('value changed');
            let sModel = this.currentStateModel;
            if (sModel) {
                sModel.stateMachine.changeState('edit');
                sModel.isDirty = true;
                if (sModel.iscomplete) { //曾經填寫完成，之後每個欄位變更，都需立即檢核
                    //validation
                    let accept = this.acceptRow(this.currentRow, true);
                }
                if (this.saveButton) {
                    Show(this.saveButton);
                }
            } else {
                console.log(`waring: input change event. this.currentStateModel is null`);
            }

        });

        $EventHandler(inputs, 'focus', (e) => {

            let elm = e.target;
            let tr = elm.closest('tr');
            if (!this.currentRow) this.currentRow = tr;

            let isCurrentRow = (tr == this.currentRow); //是否編輯中的列
            let value = GetValue(elm); //變更前的值
            elm.setAttribute('data-value-before-change', value);
            elm.setAttribute('data-is-current-row', `${isCurrentRow ? 'true' : ''}`);

            if (this.deleteButton) {
                $SetEnable(this.deleteButton);
            }

            console.log(`${e.target.tagName} focus event.`);
            if (!isCurrentRow) {
                console.log(`row changing.`);
                e.preventDefault();
                //使select的清單收起來
                Hide(elm);

                //this.focusElement.focus();  //先移走focus，防止dialog關閉後，focus自動回到相同元素，造成無窮回圈
                //validation
                let accept = this.acceptRow(this.currentRow, true);

                //重新顯示
                Show(elm);

                if (accept) {
                    this.currentRow = tr;
                    this.focusElement = elm;
                } else {
                    return;
                }
            } else {
                this.focusElement = elm;
            }
        });

        //set focus on first element
        for (const elm of inputs) {
            let type = elm.getAttribute('type');
            type = (type) ? type.toLowerCase() : '';
            let isHidden = false;
            isHidden = (elm.classList && (elm.classList.contains('hide') || elm.classList.contains('hidden')));
            if (type != 'hidden' && !isHidden) {
                let zindex = elm.getAttribute('zindex');
                if (isNaN(zindex)) {
                    //沒有設定zindex
                    elm.focus();
                    break;
                } else {
                    //若有設定，則zindex必須>=0
                    let intZindex = parseInt(zindex) || 0;
                    if (intZindex >= 0) {
                        elm.focus();
                        break;
                    } else {
                        continue;
                    }
                }
            }
        }

        //if (this.canDelete && this.deleteButton) {
        if (this.deleteButton) {
            Show(this.deleteButton);
            $SetEnable(this.deleteButton);
        }

    } else {
        //不可編輯(canEdit=false)的情況
        console.log(`changeRowMode() aborted because the canEdit property is false`);
    }

}

//初始化具狀態的資料列
$TableObject.prototype.initializeDataList = function (dataListJson, submitActionName, fieldControllerArr) {
    /*
     * dataListJson: json格式的data model array
     * submitActionName:儲存變更時所用的action name(含路徑)
     * fieldControllerArr: 欄位對應陣列[{name:"", text:"", required:""},{},...]
     */

    if (!this.submitActionName) {
        this.submitActionName = submitActionName;
    }

    //reset currentRow and focusElement
    this.currentRow = undefined;
    this.focusElement = undefined;

    //(1)設定dataList
    let dataList = ConvertToJSObject(dataListJson);
    if (dataList && Array.isArray(dataList)) {
        let rows = this.rows;
        if (rows.length != dataList.length) {
            console.log(`table id=${this.id} initializeDataList() failed.the item count of dataListJson(${dataList.length}) is not equal to the table row count(${rows.length})`);
            return;
        } else {
            //清除model list
            this.stateDataList.clear();

            for (let i = 0; i < dataList.length; i++) {
                let temp_id = $CreateTempId();
                rows[i].setAttribute('data-temp-id', temp_id);
                //每一列資料，配一個狀態機
                let stateMachine = new $StateMachine($ModelState.original);
                stateMachine.transitionRules = this.rowStateTransitionRules;
                this.stateDataList.set(temp_id, new $StateModel(dataList[i], stateMachine ,rows[i]));
            }

        }

    } else {
        if (dataList) {
            console.log(`table id=${this.id} initializeDataList() failed. dataListJson argument is not valid array. ${dataListJson}`);
        } else {
            console.log(`warning: initializeDataList() the dataListJson is null or empty.`);
        }
    }

    //(2)欄位控制設定，若有給參數，則用之；若無，則從表格欄位標頭取之。
    if (fieldControllerArr) {
        let fcArr = ConvertToJSObject(fieldControllerArr);
        if (fcArr && Array.isArray(fcArr)) {
            for (let fc of fcArr) {
                if (!fc.cellIndex) fc['cellIndex'] = 0;
                this.fieldControllers.set(fc.name, new $FieldController(fc.cellIndex, fc.name, fc.text, fc.required, fc.unique));
            }
        } else {
            console.log(`warning: initializeDataList() fieldControllerArr is not a valid js array syntax`);
        }
    } else {
        let thead = this.thead;
        if (!thead) {
            console.log(`warning: initializeDataList() thead is missing and failed to buid fieldControllers.`);
        } else {
            let ths = thead.querySelectorAll('th[data-field]');
            if (ths && ths.length > 0) {
                //先清除
                this.fieldControllers.clear();

                for (let th of ths) {
                    let cellIndex = th.cellIndex;
                    let colName = th.getAttribute('data-field');
                    let colText = th.innerText;
                    if (!colText) colText = colName;
                    let required = th.getAttribute('data-required');
                    let unique = th.getAttribute('data-unique');
                    this.fieldControllers.set(colName, new $FieldController(cellIndex, colName, colText, required, unique));
                }
            } else {
                console.log(`warning: initializeDataList() found nothing for th[data-filed] and failed to buid fieldControllers.`);
            }
        }
    }

    //(3)取的row template
    this.rowTemplate = _(`[data-template-for="${this.id}"]`);
    if (this.canEdit && !this.rowTemplate) {
        console.log(`table id=${this.id} initializeDataList() failed. row template element with [data-template-for="${this.id}"] not found.`);
    }

    //(4)綁定功能按鈕
    if (!this.isButtonBind && this.dataScopeContainer) {
        this.isButtonBind = true;
        this.addButton = this.dataScopeContainer.querySelector('[data-command="add"]');
        this.editButton = this.dataScopeContainer.querySelector('[data-command="edit"]');
        this.saveButton = this.dataScopeContainer.querySelector('[data-command="save"]');
        this.cancelButton = this.dataScopeContainer.querySelector('[data-command="cancel"]');
        this.deleteButton = this.dataScopeContainer.querySelector('[data-command="delete"]');
        this.searchButton = this.dataScopeContainer.querySelector('[data-command="search"]');

        let thisTable = this;
        if (this.addButton) {
            this.addButton.addEventListener('click', () => {
                thisTable.addRow.apply(thisTable);
            });
        } else {
            console.log(`warning: button with [data-command="add"] attribute not found within dataScopeContainer[data-scope="${this.dataScopeName}"]`);
        }

        if (this.canEdit) {
            if (this.saveButton) {
                this.saveButton.addEventListener('click', () => {
                    thisTable.submitChange.apply(thisTable);
                });
            } else {
                console.log(`warning: button with [data-command="save"] attribute not found within dataScopeContainer[data-scope="${this.dataScopeName}"]`);
            }
        }

        if (this.canDelete) {
            if (this.deleteButton) {
                this.deleteButton.addEventListener('click', () => {
                    thisTable.deleteRow.apply(thisTable);
                });
            } else {
                console.log(`warning: button with [data-command="delete"] attribute not found within dataScopeContainer[data-scope="${this.dataScopeName}"]`);
            }
        }
    }
}

//設定欄位檢核函式(callback函式傳入參數(value)，回傳格式：{pass:true/false, message:''})
$TableObject.prototype.setValidator = function (fieldName, validateCallback) {
    let fc = this.fieldControllers.get(fieldName);
    if (!fc) {
        console.log(`setValidator() failed. the fieldName=${fieldName} is not found from the fieldControllers(size=${this.fieldControllers.size}).`);
        return;
    }
    fc.validate = validateCallback;
}

//增加一列
$TableObject.prototype.addRow = function () {
    if (!this.rowTemplate) {
        console.log('addRow() failed. rowTemplate is undefined.');
        return;
    }

    //(1)先檢核current state model(mapping to currentRow)，若無問題(接受)再新增列。(***只有canEdit=true才須此檢核)
    if (this.canEdit && this.currentRow) {
        let accept = this.acceptRow(this.currentRow);
        if (!accept) return;
    }

    //(2)新增列
    let tr = document.createElement('tr');
    //tr.innerHTML = this.rowTemplate.innerHTML;
    let temp_id = $CreateTempId();
    tr.setAttribute('data-temp-id', temp_id);
    tr.setAttribute('zindex', this.rowTabIndex);
    this.tbody.appendChild(tr);

    let stateMachine = new $StateMachine('new', 'client');//state=new
    stateMachine.transitionRules = this.rowStateTransitionRules;
    this.stateDataList.set(temp_id, new $StateModel({}, stateMachine, tr)); 
    this.currentRow = tr;
    this.focusElement = undefined;
    if (this.canEdit) {
        //進入編輯模式
        this.changeRowMode(tr);
    } else {
        //非編輯模式(略過)
        console.log(`addRow() aborted because canEdit property is false`);
    }

    Hide(this.noData);

}

//檢核，若通過則接受輸入的資料並更新model值，回傳true, 否則回傳false
$TableObject.prototype.acceptRow = function (row, moveFocus) {
    /*
     * moveFocus: 若有跳窗訊息時，是否要先將focus移走(至this.focusElement)，防止dialog關閉後，focus自動回到相同元素，造成無窮回圈
     */

    if (!row) {
        console.log(`acceptRow() failed. row is null`);
        return false;
    }
    let temp_id = row.getAttribute('data-temp-id');
    let sModel = this.stateDataList.get(temp_id);

    try {
        this.clearValidationHint();

        if (sModel) {
            if (sModel.state == $ModelState.new || sModel.state == $ModelState.updating) {
                let result = this.validateRow(row);
                if (!result.pass) {
                    let msg = { msgType: 'warning', caption: '系統提示', message: result.message };
                    if (moveFocus) {
                        if (this.focusElement) {
                            this.focusElement.focus(); 
                        }
                    }

                    //問題欄位highlight
                    this.setValidationHint(row, result.hintArr);

                    //存在欄位highlight
                    if (result.existingArr && result.existingArr.length > 0) {
                        let existRow = result.existingArr[0].row;
                        let fieldNameArr = result.existingArr.map(x=> x.fieldName);
                        this.highlightExistingFields(existRow, fieldNameArr);
                    }

                    $PopMessage(msg);
                    return false;
                }

                //(1-2)檢核通過才能將欄位值回存
                $UpdateModel(sModel.model, row);

                //(1-3)變更model狀態(「接受」檢核過的資料)
                sModel.stateMachine.changeState('accept');
            }
            sModel.iscomplete = true; //接受表示欄位已填寫完成，之後任何欄位的變更都需立即檢核
            return true;
        } else {
            console.log(`acceptRow() failed. temp_id=${temp_id} not found in stateDataList`);
            return false;
        }
    } catch (e) {
        let msg = { msgType: 'error', caption: '系統提示', message: e.message };
        $PopMessage(msg);
        return false;
    }

}

//刪除列
$TableObject.prototype.deleteRow = async function (deleteActionUrl, rowExpresionFields) {
    //To do: this function has not completed yet
    /*
     * if deleteActionUrl is given, then call server api first and then remove client row after success,
     * if no deleteActionUrl, just remove client row
     */

    if (!this.currentRow) {
        let msg = { msgType: 'info', caption: '系統提示', message: '請先點選要刪除的資料列' };
        $PopMessage(msg);
        return;
    }

    let $row = _$(this.currentRow);
    let stateModel = this.currentStateModel;
    let rowModel = stateModel.model;

    //區分client region / server region
    let region = stateModel.stateMachine.region;
    if (region != 'client') {
        if (deleteActionUrl) {
            if (!rowExpresionFields) rowExpresionFields = GetRowDataKey($table.currentRow);
            let rowExprFieldArr = rowExpresionFields.split(',').map(x=>x.trim());
            let message = '確定要刪除[';
            let fieldCount = 0;
            for (const fieldName of rowExprFieldArr) {
                fieldCount++;
                let fc = this.fieldControllers.get(fieldName);
                let text = $row.child(`[data-field="${fieldName}"]`)?.text() ?? '';
                if (!text) text = $row[0].cells[fc.cellIndex]?.innerText?.trim()??'';
                if (fieldCount > 1) message += ',';
                message += `<b>${fc.fieldText}</b>=<span class="text-red">${text}</span>`;
            }
            message += ']這筆資料？';

            let msg = { msgType: 'warning', caption: '系統提示', message: message };
            let confirm = await $PopConfirm(msg);
            if (confirm) {
                let url = deleteActionUrl;
                let postData = ConvertToFormData(rowModel);
                let json = await FetchJson(url, 'POST', postData);
                if (json.success) {

                } else {
                    ShowFetchJsonFailMessage(null, json);
                    return;
                }
            } else {
                return;
            }
        } else {
            console.log(`warning:deleteRow(), server record not deleted because of missing the deleteActionUrl argument. Note: However, the row in the client table would be removed.`);
        }
    }

    //remove client row and data model
    this.currentRow = undefined;
    let temp_id = $row[0].getAttribute('data-temp-id');
    $row[0].remove();
    this.stateDataList.delete(temp_id)

    let msg = { msgType: 'info', caption: '系統資訊', message: '刪除完成' };
    $PopMessage(msg);

}

//設定欄位檢核錯誤提示css
$TableObject.prototype.setValidationHint = function (row, hintArr) {
    if (!hintArr || hintArr.length == 0) return;

    //暫存此列，之後要清除醒目提示時會用到
    this.highlightRows.push(row);

    //設定model-error flag
    SetFlagOn(row, 'model-error');

    let inputs = row.querySelectorAll('input,select,textarea');
    let firstFoundElm; //第1個顯示錯誤訊息的欄位
    let fieldName;
    for (let elm of inputs) {
        fieldName = elm.getAttribute('data-field');
        let hintIndex = hintArr.findIndex(x => x.fieldName == fieldName);
        if (hintIndex < 0) continue;

        if (!firstFoundElm) firstFoundElm = elm;

        let hint = hintArr[hintIndex];
        let parent = elm.parentElement;
        if (hint && hint.message) {
            //form-input項目加上紅色外框、黃底色
            elm.classList.add('form-input-highlight');
            //設定form-input的上層元素form-item下方顯示提示文字
            if (parent) {
                parent.setAttribute('data-hint', hint.message);
            }
        }
        else {
            //若無提示訊息，則給一個全形空白，使欄位垂直對齊
            if (parent) {
                parent.setAttribute('data-hint', '');
                parent.classList.remove('form-input-highlight');
            }
        }

        //每次顯示hint之後即可移除該項
        hintArr.splice(hintIndex, 1);
    }
}

//設定已存在欄位醒目提示css
$TableObject.prototype.highlightExistingFields = function (row, fieldNameArr) {
    if (!row || !fieldNameArr || fieldNameArr.length == 0) return;

    //暫存此列，之後要清除醒目提示時會用到
    this.existingRows.push(row);

    let inputs = row.querySelectorAll('input,select,textarea,td');
    let fieldName;
    for (let elm of inputs) {
        fieldName = elm.getAttribute('data-field');
        let index = fieldNameArr.indexOf(fieldName);
        if (index < 0) continue;
        //form-input項目加上綠底色
        elm.classList.add('existing-field');

        //每次顯示hint之後即可移除該項
        fieldNameArr.splice(index, 1);
    }
}

//清除欄位檢核錯誤提示css
$TableObject.prototype.clearValidationHint = function () {
    if (!this.highlightRows || this.highlightRows.length == 0) return;
    //(1)清除檢核提示
    let row = this.highlightRows.pop();
    while (row) {
        try {
            //取消model-error flag
            SetFlagOff(row, 'model-error');

            let inputs = row.querySelectorAll('input,select,textarea');
            if (!inputs || inputs.length == 0) {
                row = this.highlightRows.pop();
                continue;
            }
            for (const elm of inputs) {
                //項目去除紅色外框和黃底色
                elm.classList.remove('form-input-highlight');
                //parent container去除下方提示文字
                let parent = elm.parent;
                if (parent) {
                    parent.setAttribute('data-hint', '');
                }
            }
        } catch (e) {
            console.log(`clearValidationHint() for highlightRows failed. error=${e.message}`);
            break;
        }

        row = this.highlightRows.pop();
    }

    //(2)清除重複的既存欄位
    row = this.existingRows.pop();
    while (row) {
        try {
            let inputs = row.querySelectorAll('input,select,textarea,td');
            if (!inputs || inputs.length == 0) {
                row = this.existingRows.pop();
                continue;
            }
            for (const elm of inputs) {
                //項目去除綠底色
                elm.classList.remove('existing-field');
            }
        } catch (e) {
            console.log(`clearValidationHint() for existingRows failed. error=${e.message}`);
            break;
        }

        row = this.existingRows.pop();
    }

}

//檢核表格列
$TableObject.prototype.validateRow = function (row) {
    if (row) {
        let inputs = row.querySelectorAll('[data-field]');
        if (!inputs || inputs.length == 0) {
            console.log(`warning: validateRow() is passed while no input element with [data-field] attribute found`);
            return { pass: true, message: '' };
        }
        let hintArr = []; //檢核欄位醒目提示
        let msgArr = []; //檢核結果訊息
        let existingArr = []; //檢核唯一性，重複時，現存欄位
        //(1)check required fields
        for (let input of inputs) {
            let vt = GetValueText(input);
            let fieldName = input.getAttribute('data-field');
            let fc = this.fieldControllers.get(fieldName);
            if (fc) {
                if (fc.required) {
                    if (!vt.value && (vt.value !== 0 && vt.value !== '0')) { //0:算有填
                        hintArr.push({ fieldName: fieldName, message: '未填寫' });
                        msgArr.push(`<b class="bold-600">${fc.fieldText}</b>:<span class="text-blue">未填寫</span>`);
                    }
                }
            } else {
                console.log(`warning: validateRow() missing fieldController of fieldName=${fieldName}`);
            }

        }

        if (hintArr.length > 0) {
            let message = msgArr.join(',&nbsp');
            return { pass: false, message, hintArr };
        }

        //(2)check unique fields
        let rowTempId = row.getAttribute('data-temp-id');
        for (let input of inputs) {
            let vt = GetValueText(input);
            if (vt.value && typeof vt.value != 'boolean' ) vt.value = vt.value.trim();
            let fieldName = input.getAttribute('data-field');
            let fc = this.fieldControllers.get(fieldName);
            if (fc.unique) {
                //檢核唯一性
                let checkResult = this.checkFieldValueUnique(rowTempId, fieldName, vt.value);
                if (!checkResult.unique) {
                    //select元素要顯示選項「文字」較好理解
                    hintArr.push({ fieldName: fieldName, message: `欄位值=${vt.text}已存在，不可重複。` });
                    msgArr.push(`<b class="bold-600">${fc.fieldText}</b>:<span class="text-blue">欄位值=${vt.text}</span>,<span class="text-red">已存在，不可重複。</span>`);
                    existingArr.push({ row: checkResult.existingRow , fieldName:fieldName});
                }
            }
        }

        if (hintArr.length > 0) {
            let message = msgArr.join(',&nbsp');
            return { pass: false, message, hintArr, existingArr };
        }

        //(3)check欄位群組唯一性
        if (this.uniqueFieldGroups && this.uniqueFieldGroups.length > 0) {
            let fieldNameArr = [];
            let valueArr = [];
            let textArr = [];
            for (const fg of this.uniqueFieldGroups) {
                //拆欄位
                fieldNameArr = fg.split(',').map(x => x.trim());
                //找欄位值
                for (const fn of fieldNameArr) {
                    let elm = row.querySelector(`[data-field="${fn}"]`);
                    if (elm) {
                        let vt = GetValueText(elm);
                        if (vt.value && typeof vt.value != 'boolean') vt.value = vt.value.trim();
                        valueArr.push(vt.value);
                        textArr.push(vt.text);
                    } else {
                        console.log(`warning: validateRow() data-field="${fn}" not found for uniqueFieldGroups=${fg}`);
                        valueArr.push('');
                        textArr.push('');
                    }
                }

                //檢核唯一性
                let checkResult = this.checkFieldGroupValueUnique(rowTempId, fieldNameArr, valueArr);
                if (!checkResult.unique) {
                    for (let i = 0; i < fieldNameArr.length; i++) {
                        hintArr.push({ fieldName: fieldNameArr[i], message: `欄位群組已存在，不可重複。` });
                        let fc = this.fieldControllers.get(fieldNameArr[i]);
                        msgArr.push(`<b class="bold-600">${fc.fieldText}</b>=<span class="text-blue">${textArr[i]}</span>`);
                        existingArr.push({ row: checkResult.existingRow, fieldName: fieldNameArr[i] });
                    }
                }
            }
        }

        if (hintArr.length > 0) {
            let message = msgArr.join(',&nbsp') + ',<span class="text-red">欄位群組已存在，不可重複。</span>';
            return { pass: false, message, hintArr, existingArr };
        }

        let model = $RetriveModel(row);  //取得目前表格列欄位所能產生的data model
        //(4)custom field validator(client javascript function)
        for (let i of inputs) {
            let fieldName = i.getAttribute('data-field');
            let fc = this.fieldControllers.get(fieldName);
            if (fc.validate) {
                let value = GetValue(i);
                let result = fc.validate.apply(null, [value, fc, model]);
                if (!result.pass) {
                    if (!result.hintArr) {
                        result.hintArr = [{ fieldName: fc.fieldName, message: result.message }];
                    }
                    return result;
                }
            }
        }

        //(5)server side validation
        /***TO DO:***/

        return { pass: true, message: '' };

    } else {
        console.log(`warning: validateRow() is passed while row argument is undefined`);
        return { pass: true, message: '' };
    }
}

//檢核欄位值是否重複(return true:唯一，false:重複)
$TableObject.prototype.checkFieldValueUnique = function (rowTempId, fieldName, value) {
    if (this.stateDataList.size == 0) return true;

    let items = [...this.stateDataList.entries()];

    for (let [k, v] of items) {
        if (k == rowTempId) continue; //比對時略過自己這一筆
        if (v.model[fieldName] == value) {
            return { unique: false, existingRow: v.bindingElement };
        }
    }
    return { unique: true, existingRow: undefined };
}

//檢核欄位組合值是否重複(return true:唯一，false:重複)
$TableObject.prototype.checkFieldGroupValueUnique = function (rowTempId, fieldNameArr, valueArr) {
    if (this.stateDataList.size == 0) return true;

    let items = [...this.stateDataList.entries()];

    for (let [k, v] of items) {
        if (k == rowTempId) continue; //比對時略過自己這一筆
        let same = true;
        for (let i = 0; i < fieldNameArr.length; i++) {
            same = same && (v.model[fieldNameArr[i]] == valueArr[i]);
            if (!same) break;
        }

        if (same) {
            return { unique: false, existingRow: v.bindingElement };
        }
    }
    return { unique: true, existingRow: undefined };
}

//完成資料列編輯，回傳完成狀態(true/false)
$TableObject.prototype.complete = function () {
    if (!this.currentRow) return true;
    let accept = this.acceptRow(this.currentRow);
    return accept;
}

//送出變更的資料集
$TableObject.prototype.submitChange = async function () {
    let complete = $table.complete();
    if (!complete) return;

    let changeList = this.changeList;
    if (!changeList || changeList.length == 0) {
        let msg = { msgType: 'info', message: '無資料變更需要儲存' };
        $PopMessage(msg);
        return;
    }

    if (!this.submitActionName) {
        let msg = { msgType: 'error', message: `儲存失敗！未設定table(id=${this.id})的submitActionName` };
        $PopMessage(msg);
        return;
    }

    let jsonContent = JSON.stringify(changeList);
    let postData = { json: jsonContent };
    let json = await FetchJson(this.submitActionName, 'POST', postData);
    if (json.success) {
        let msg = { msgType: 'info', message: '儲存成功！' };
        $PopMessage(msg);
        $table.renewState(); //更新資料列狀態(成original)
    } else {
        ShowFetchJsonFailMessage(null, json);
    }
}

//更新stateDataList的全部列狀態(1)將deleted的列移除、將added和updated的列設定成orignal (2)將所有region設成'none'
$TableObject.prototype.renewState = function () {
    if (!this.stateDataList || this.stateDataList.size == 0) return;
    this.stateDataList.forEach((value, key, map) => {
        value.region = 'none';
        if (value.state == 'deleted') {
            map.delete(key);
        }
        if (value.state == 'added' || value.state == 'updated') {
            value.state = 'original';
        }
    });
    //let list = [...this.stateDataList.entries()];
    //for (let [k, v] of list) {
    //    if (v.state == 'deleted') {
    //        this.stateDataList.delete(k);
    //    }
    //    if (v.state == 'added' || v.state == 'updated') {
    //        v.state == 'original';
    //    }
    //}
}

//設定(更新)tbody內容，並註冊列選取和列命令事件處理
$TableObject.prototype.setBodyContent = function (innerHTML, modelList) {
    /*
     * innerHTML: tbody的html
     * modelList: 表格列所對應的model list json
     */

    if (this.table.tBodies.length = 0) {
        this.table.appendChild(document.createElement('tbody'));
    }

    //(1)更新表格列內容
    this.table.tBodies[0].innerHTML = innerHTML;

    //(2)更新modelList
    if (modelList) {
        this.initializeDataList(modelList);
    }

    //更新(無資料)可視性
    this.refreshNoData();

    //註冊列選取事件處理
    this.setRowSelectHandler();
    //if (this.rowSelectEventHandler) {
    //    //RegisterTableRowSelectedEvent(this.id, this.rowSelectEventHandler);
        
    //}

    //註冊列命令事件處理(此事件處理註冊在table上，而非row上)，若已註冊，不須重複註冊
    if (!this.rowCommandHandlerRegistered && this.rowCommandEventHandler) {
        //RegisterTableRowCommandEvent(this.id, this.rowCommandEventHandler);
        this.setRowCommandHandler();
    }

    //當表格狀態是縮至側邊面版時，隱藏部分「可隱藏的」欄位
    if (this.rwdState == 'side') {
        this.hidePartialColumns();
    }

    //if (this.rows && this.rows.length > 0) {
    //    Hide(this.noData);
    //} else {
    //    Show(this.noData);
    //}
}

//清除tbody內容
$TableObject.prototype.clearBodyContent = function () {

    //清除表格列內容
    this.table.tBodies[0].innerHTML = '';

    //清除model list
    this.stateDataList.clear();

    //reset currentRow and focusElement
    this.currentRow = undefined;
    this.focusElement = undefined;

    //更新(無資料)可視性
    this.refreshNoData();
}

//更新(無資料)可視性
$TableObject.prototype.refreshNoData = function () {
    //若資料筆數為0，則顯示無資料訊息
    if (this.hasDataRow) {
        Hide(this.noData); 
    } else {
        Show(this.noData);
    }
}

//更新分頁按鈕列
$TableObject.prototype.refreshPagingBar = function (pagingSetting) {
    for (const bar of $PagingBars) {
        if (bar.bindingTargetId == this.id) {
            bar.set(pagingSetting);
        }
    }
}

//不顯示欄位標頭
$TableObject.prototype.hideColumnHeader = function () {
    if (this.thead) {
        Hide(this.thead);
    }
}

//當表格縮至側邊面版時，隱藏部分「可隱藏的」欄位
$TableObject.prototype.hidePartialColumns = function () {
    //let canHideCols = this.table.querySelectorAll('thead > th[data-canHde="true"]');
    let canHideCols = this.getPartialColumns();
    if (!canHideCols || canHideCols.length == 0) return;
    let canHideColNames = [...canHideCols].map(x => x.getAttribute('data-column-name') || x.innerText);
    this.hideColumn(canHideColNames);
}

//顯示被隱藏的「可隱藏的」欄位
$TableObject.prototype.showPartialColumns = function () {
    //let canHideCols = this.table.querySelectorAll('thead > th[data-canHde="true"]');
    let canHideCols = this.getPartialColumns();
    if (!canHideCols || canHideCols.length == 0) return;
    let canHideColNames = [...canHideCols].map(x => x.getAttribute('data-column-name') || x.innerText);
    this.showColumn(canHideColNames);
}

//取得當表格縮至側邊面版時，「可隱藏的」欄位
$TableObject.prototype.getPartialColumns = function () {
    let canHideCols = [];
    if (this.table.rows.length > 0) {
        let allCols = this.table.rows[0].querySelectorAll('th');
        if (allCols && allCols.length > 0) {
            for (const col of allCols) {
                let rwdLevel = col.getAttribute('data-column-rwd-level');
                if (rwdLevel && rwdLevel > this.rwdCutLevel) {
                    canHideCols.push(col);
                }
            }
        }

    }

    return canHideCols;
}


//取得表頭列欄位(<th>)
$TableObject.prototype.getHearderColumns = function (column_names) {
    /*
     * column_names: (1) 'colName_1, colName_2,...' , or (2) {colName_1, colName2, ...}
     * 如果未給column_names參數，則傳回全部表頭欄位
     */
    if (!this.valid()) return undefined;
    let thCols = [];
    let found;
    let filtered = [];
    if (this.table.rows.length > 0) {
        found = this.table.rows[0].querySelectorAll('th');
        if (found && found.length > 0) thCols.push(...found);
    }
    if (this.table.rows.length > 1) {
        found = this.table.rows[1].querySelectorAll('th');
        if (found && found.length > 0) thCols.push(...found);
    }
    if (!column_names) {
        return thCols;
    } else {
        let colNameArr = GetColumnNameArray(column_names); //轉成陣列
        if (!colNameArr) {
            console.log(`getHearderColumns() failed. invalid argument of ${column_names}`);
            return undefined;
        } else {
            thCols.forEach(th => {
                if (colNameArr.includes(th.innerText)
                    || colNameArr.includes(th.getAttribute('data-column-name'))) {
                    filtered.push(th);
                }
            });
        }
        return filtered;
    }

};

//取得css nth-child所對應的欄位index, index-base:1
$TableObject.prototype.getCssNthColumnIndexs = function (column_names_or_id_or_index_array) {
    /*
     * column_names_or_id_or_index_array: 格式1: 'colName_1, colName_2,...' , or ['colName_1', 'colName2', ...]
     * column_names_or_id_or_index_array: 格式2: '1,2,...' , or [1, 2, ...]
     */

    if (!this.valid()) return undefined;

    let colArgs = GetColumnNameArray(column_names_or_id_or_index_array); //轉成陣列
    if (!colArgs || colArgs.length == 0) return undefined;
    let found = this.table.rows[0].querySelectorAll('th');
    let allColArr;
    if (found && found.length > 0) {
        allColArr = [...found];
    }
    if (!allColArr) return undefined;

    let allColNames = allColArr.map(x => x.getAttribute('data-column-name') || x.innerText);
    let indexArr = [];
    let foundIndex = -1;
    let adjustIndex = -1;
    colArgs.forEach(c => {
        if (isNaN(c)) {
            foundIndex = allColNames.findIndex(x => x == c); //base:0
            adjustIndex = foundIndex + 1;
            if (foundIndex >= 0 && indexArr.includes(adjustIndex) == false) {
                indexArr.push(adjustIndex); //base:1
            }
        } else {
            adjustIndex = parseInt(c);
            if (isNaN(adjustIndex)==false && indexArr.includes(adjustIndex) == false) {
                indexArr.push(adjustIndex);
            }
        }
    });
    return indexArr.sort();
}
//顯示表格欄位, index-base:1
$TableObject.prototype.showColumn = function (column_names_or_id_or_index_array) {
    /*
     * column_names_or_id_or_index_array: 格式1: 'colName_1, colName_2,...' , or ['colName_1', 'colName2', ...]
     * column_names_or_id_or_index_array: 格式2: '1,2,...' , or [1, 2, ...]
     * 如果未給column_names參數，則顯示全部欄位
     */

    let args = column_names_or_id_or_index_array;
    if (!args) {
        console.log('showColumn() failed. the column_names_or_id_or_index_array argument is not given.');
        return;
    }
    let cssNthIndex = this.getCssNthColumnIndexs(args);
    if (!cssNthIndex || cssNthIndex.length == 0) return;
    let selector_th, selector_td;
    cssNthIndex.forEach(i => {
        selector_th = `thead tr th:nth-child(${i})`;
        selector_td = `tbody tr td:nth-child(${i})`;
        let ths = this.table.querySelectorAll(selector_th);
        let tds = this.table.querySelectorAll(selector_td);
        if (ths && ths.length > 0) {
            for (const th of ths) Show(th);
        }
        if (tds && tds.length > 0) {
            for (const td of tds) Show(td);
        }
    });


    //let thArr = this.getHearderColumns(column_names_or_id_or_index_array);
    //if (thArr && thArr.length > 0) {
    //    thArr.forEach(th => {
    //        Show(th);
    //    });
    //}

};
//隱藏表格欄位, index-base:1
$TableObject.prototype.hideColumn = function (column_names_or_id_or_index_array) {
    /*
     * column_names_or_id_or_index_array: 格式1: 'colName_1, colName_2,...' , or ['colName_1', 'colName2', ...]
     * column_names_or_id_or_index_array: 格式2: '1,2,...' , or [1, 2, ...]
     */
    let args = column_names_or_id_or_index_array;
    if (!args) {
        console.log('hideColumn() failed. the column_names_or_id_or_index_array argument is not given.');
        return;
    }
    let cssNthIndex = this.getCssNthColumnIndexs(args);
    if (!cssNthIndex || cssNthIndex.length == 0) return;
    let selector_th, selector_td;
    cssNthIndex.forEach(i => {
        selector_th = `thead tr th:nth-child(${i})`;
        selector_td = `tbody tr td:nth-child(${i})`;
        let ths = this.table.querySelectorAll(selector_th);
        let tds = this.table.querySelectorAll(selector_td);
        if (ths && ths.length > 0) {
            for (const th of ths) Hide(th);
        }
        if (tds && tds.length > 0) {
            for (const td of tds) Hide(td);
        }
    });

    //let thArr = this.getHearderColumns(column_names);
    //if (thArr && thArr.length > 0) {
    //    thArr.forEach(th => {
    //        Hide(th);
    //    });
    //}

};


//Lighter Table物件集合
let $Tables = []; /***Global***/

//Lighter Table Object Short Syntax
function $Table(table_or_id, rowSelectEventHandler, rowCommandEventHandler, canEdit) {
    let table = $GetElement(table_or_id);
    let index = $Tables.findIndex(x => x.id == table.id);
    let tableObject;
    if (index < 0) {
        tableObject = new $TableObject(table_or_id, rowSelectEventHandler, rowCommandEventHandler, canEdit);
        let length = $Tables.push(tableObject);

    } else {
        tableObject = $Tables[index];
    }
    return tableObject;
}


/*表格選取的列model */
function TableSelectedRow() {
    this.table_id = '';
    this.selected_tr = undefined;
    this.row_data_key = '';
    this.command_name = '';
    this,dataKeyStr = '';        //只有鍵值欄位的「值」多組以逗號分隔
    this.dataKeyObj = {};        //有鍵值欄位的「欄名」和「值」的物件形式
    this.dataModel = null;       //表格列所對應的data model
}

//表格選取的列model的暫存器
let $SelectedRows = []; /***Global***/
function SetTableSelectedRow(rowModel) {
    if (!rowModel || !rowModel.table_id) {
        console.log(`SetTableSelectedRow():invalid rowModel=${rowModel}`);
        return;
    }
    let index = $SelectedRows.findIndex(x => x.table_id == rowModel.table_id);

    let item;
    if (index < 0) {
        let length = $SelectedRows.push(rowModel);
        item = rowModel;
    } else {
        item = $SelectedRows[index];
        if (item.selected_tr) {
            item.selected_tr.classList.remove('selected');
        }
        $SelectedRows[index] = rowModel;
        item = rowModel;
    }
    if (item.selected_tr) {
        item.selected_tr.classList.add('selected');
    }
}

//設定列選取事件處理函式(舊版相容)
function RegisterTableRowSelectedEvent(tableOrId, rowSelectHandler) {
    /*
     * tableOrId: table物件或 id 屬性
     * rowSelectHandler: 列選取事件handler
     */

    let table = $GetElement(tableOrId);
    if (!table) {
        console.log(`RegisterTableRowSelectedEvent() failed. tableOrId=${tableOrId} not found`);
        return;
    }
    tableId = `#${table.id}`;

    let __targets = table.querySelectorAll(`tbody>tr`);
    if (!__targets || __targets.length == 0) return;
    for (const tr of __targets) {
        tr.addEventListener('focus', function (e) {
            tr.blur();
            let row_data_key = GetRowDataKey(tr);

            let model = new TableSelectedRow();
            model.table_id = tableId;
            model.selected_tr = tr;
            model.row_data_key = row_data_key;
            model.command_name = 'select';
            //保存選取列
            SetTableSelectedRow(model);

            rowSelectHandler(row_data_key);
            //e.stopPropagation(); //停止event bubbling
        });
    }
}

//設定列操作命令(例如：edit, delete...)事件處理函式
function RegisterTableRowCommandEvent(tableOrId, rowCommandHandler) {
    /*
     * tableOrId: table物件或 id 屬性
     * rowCommandHandler: 列命令按鈕點擊事件handler
     */

    let tableObj = $Table(tableOrId);
    //let table = $GetElement(tableOrId);
    if (tableObj) {
        if (!tableObj.rowCommandEventHandler && rowCommandHandler) {
            tableObj.rowCommandEventHandler = rowCommandHandler;
        }

        if (!tableObj.rowCommandHandlerRegistered && tableObj.rowCommandEventHandler) {

            tableObj.rowCommandHandlerRegistered = true;

            tableObj.table.addEventListener('click', function (e) {

                //判斷是否命令按鈕
                let src = e.target;
                if (src.matches("[data-command]")) {
                    //找出所在的列
                    let tr = FindParent(src, 'tr', 4);
                    tableObj.currentRow = tr;

                    let args = new TableSelectedRow();
                    args.table_id = tableObj.id;
                    args.selected_tr = tr;
                    args.dataKeyStr = GetRowDataKey(tr);        //只有鍵值欄位的「值」多組以逗號分隔
                    args.dataKeyObj = GetRowDataKeyObject(tr);  //有鍵值欄位的「欄名」和「值」的物件形式
                    args.row_data_key = args.dataKeyStr;        //for舊版相容
                    args.dataModel = tableObj.currentRowModel;      //表格列所對應的data model
                    let command_name = src.getAttribute('data-command');
                    args.command_name = command_name ? command_name : '';
                    //保存選取列
                    SetTableSelectedRow(args);
                    //觸發handler
                    //rowCommandHandler(model);
                    tableObj.rowCommandEventHandler.apply(null, [args]);
                }
                e.stopPropagation(); //停止event bubbling
            });
        } else {
            console.log(`RegisterTableRowCommandEvent() aborted because rowCommandEventHandler has already been registered.`);
        }

    } else { 
        //以下for舊版相容
        let table = $GetElement(tableOrId);
        if (!table) {
            console.log(`RegisterTableRowCommandEvent() failed. tableOrId=${tableOrId} not found`);
            return;
        }
        tableId = `#${table.id}`;

        table.addEventListener('click', function (e) {

            //判斷是否命令按鈕
            let src = e.target;
            if (src.matches("[data-command]")) {
                //找出所在的列
                let tr = FindParent(src, 'tr', 4);
                let model = new TableSelectedRow();
                model.table_id = tableId;
                model.selected_tr = tr;
                model.row_data_key = GetRowDataKey(tr);
                let command_name = src.getAttribute('data-command');
                model.command_name = command_name ? command_name : '';
                //保存選取列
                SetTableSelectedRow(model);
                //觸發handler
                rowCommandHandler(model);
            }
            e.stopPropagation(); //停止event bubbling
        });
    }

}

function ClearTableRowSelectedEvent(tableId, rowSelectHandler) {
    /*
     * tableId: table id 屬性
     * rowSelectHandler: 列選取事件handler
     */
    if (!tableId.startsWith('#')) tableId = '#' + tableId;
    let __targets = document.querySelectorAll(`${tableId}>tbody>tr`);

    if (!__targets || __targets.length == 0) return;
    for (const t of __targets) {
        t.removeEventListener('focus', rowSelectHandler);
    }
}

//保存表格選定列資料暫存
function SetSelectedRowData(tableId, selectedRow) {
    /*
     * tableId:目標table的id
     * selectedRow:選定列的innerHTML(全部td內容含tag)
     */
    tableId = tableId.startsWith('#') ? tableId : `#${tableId}`;
    let table = document.querySelector(tableId);
    if (!table) {
        console.log(`table[${table}] not found`);
        return;
    }
    let store = table.parentElement.querySelector("table.selected-row-store");
    let tr;
    if (!store) {
        store = document.createElement('table');
        table.after(store);
        tr = store.appendChild(document.createElement('tr'));
        store.classList.add('selected-row-store', 'hide', 'hidden');
        store.setAttribute('style', 'display:none');
    } else {
        tr = store.firstChild;
    }
    tr.innerHTML = selectedRow;

}

//取得table row的data-key屬性值
function GetRowDataKey(row) {
    if (!row) return '';
    let key;
    try {
        key = row.getAttribute('data-key');
        if (!key) key = '';
    } catch (e) {
        console.log('GetRowDataKey() failed.' + e);
        key = '';
    }
    return key;
}

//取的table row 所對應的鍵值物件
function GetRowDataKeyObject(row) {
    if (!row) return {};
    let keyNames = row.getAttribute('data-key-field');
    let keyValues = row.getAttribute('data-key');
    if (!keyNames) {
        console.log(`GetRowDataKeyObject() failed. [data-key-field](key name) attribute of table row has no value.`);
    }
    if (!keyValues) {
        console.log(`GetRowDataKeyObject() failed. [data-key](key value) attribute of table row has no value.`);
    }

    let keyNameArr = keyNames.split(',').map(x => x.trim());
    let keyValueArr = keyValues.split(',').map(x => x.trim());
    let keyObj = {};
    try {
        for (let i = 0; i < keyNameArr.length; i++) {
            keyObj[keyNameArr[i]] = keyValueArr[i];
        }
    } catch (e) {
        console.log(`GetRowDataKeyObject() failed. error=${e.message}`);
    }
    return keyObj;
}

//保存表格選定列資料鍵值
function SetSelectedRowKey(tableId, selectedRowKey) {
    /*
 * tableId:目標table的id
 * selectedRowKey:選定列的所對應的資料鍵值
 */
    tableId = tableId.startsWith('#') ? tableId : `#${tableId}`;
    let table = document.querySelector(tableId);
    if (!table) {
        console.log(`table[${table}] not found`);
        return;
    }
    let store = table.parentElement.querySelector("div.selected-row-key");
    if (!store) {
        store = document.createElement('div');
        table.after(store);
        store.classList.add('selected-row-key', 'hide', 'hidden');
        store.setAttribute('style', 'display:none');
    }
    store.textContent = selectedRowKey;
}

//取得表格選定列資料暫存
function GetSelectedRowData(tableId) {
    /*
     * tableId:目標table的id
     */
    tableId = tableId.startsWith('#') ? tableId : `#${tableId}`;
    let table = document.querySelector(tableId);
    if (!table) {
        console.log(`table[${table}] not found`);
        return;
    }
    let store = table.parentElement.querySelector("table.selected-row-store");
    if (!store) return '';
    return store.firstChild.innerHTML;
}

//取得表格選定列資料鍵值
function GetSelectedRowKey(tableId) {
    /*
     * tableId:目標table的id
     */
    tableId = tableId.startsWith('#') ? tableId : `#${tableId}`;
    let table = document.querySelector(tableId);
    if (!table) {
        console.log(`table[${table}] not found`);
        return;
    }
    let store = table.parentElement.querySelector("div.selected-row-key");
    if (!store) return '';
    return store.textContent;
}

//清除表格選定列資料暫存
function ClearSelectedRowData(table_or_id) {
    /*
     * tableId:目標table的id或元素物件
     */
    let table = $GetElement(table_or_id);
    if (!table) {
        console.log(`table[${table}] not found`);
        return;
    }

    let store = table.parentElement.querySelector("table.selected-row-store");
    if (!store) return;
    store.firstChild.innerHTML = '';
}

//取得表格欄位名稱陣列
function GetColumnNameArray(column_names_or_id_or_index_array) {
    if (!column_names_or_id_or_index_array) return undefined;

    let arg = column_names_or_id_or_index_array;
    let argType = typeof arg;
    if (argType === 'string') {
        //字串
        return arg.split(',').map(x => x.trim());;
    } else if (argType === 'object') {
        if (arg instanceof Element) {
            //html物件
            if (arg.tagName == 'TABLE') {
                let ths = arg.querySelectorAll('th');
                if (ths && ths.length > 0) {
                    let colNames = [];
                    for (const th of ths) {
                        let attrValue = th.getAttribute('data-column-name');
                        if (attrValue) {
                            colNames.push(attrValue);
                        }
                    }

                    if (colNames.length == 0) {
                        console.log('GetColumnNameArray() failed. no data-column-name attribute has been set for table th tags.');
                        return undefined;
                    }
                    return colNames;
                }
            } else {
                console.log(`GetColumnNameArray() failed. the given element is not a table.`);
                return undefined;
            }
        } else {
            if (Array.isArray(arg)) {
                //陣列
                return arg;
            } else {
                //js物件
                return Object.keys(arg);
            }
        }
    }
    return undefined;
}


//預覽選取的圖片檔
function PreviewFileInputImage(file_or_id, img_element_or_id) {
    if (!file_or_id) return;
    if (!img_element_or_id) return;
    let input = $GetElement(file_or_id);
    let img = $GetElement(img_element_or_id);
    if ((!input) || (!img)) return;
    if (input instanceof File) {
        let reader = new FileReader();
        reader.readAsDataURL(input);
        reader.onload = function () {
            if (reader.readyState == 2) {
                img.src = reader.result;
                Show(img);
            }
        }
    }
    else if (input.files) {
        let reader = new FileReader();
        reader.readAsDataURL(input.files[0]);
        reader.onload = function () {
            if (reader.readyState == 2) {
                img.src = reader.result;
                Show(img);
            }
        }
    } else {
        img.src = "";
        Hide(img);
    }

}

//水平滑動區塊
function $Sliding_H(element_or_id, width) {
    if (!element_or_id) return;
    let elm = $GetElement(element_or_id);
    if (!elm) {
        console.log(`$Sliding_H() failed.${element_or_id} not found`);
        return;
    }
    if (width.endsWith('%')==false && isNaN(width) == false) width += 'px';
    //elm.style.width = width + ' !important';
    elm.style.setProperty('width',width,'important');
}

//垂直滑動區塊
function $Sliding_V(element_or_id, height) {
    if (!element_or_id) return;
    let elm = $GetElement(element_or_id);
    if (!elm) {
        console.log(`$Sliding_V() failed.${element_or_id} not found`);
        return;
    }
    if (height.endsWith('%') == false && isNaN(height) == false) height += 'px';
    let visible = (height != '0%' && height != '0px');
    if (visible) {
        let child = elm.firstElementChild;
        if (child) {
            Show(child);
        }
    }

    //elm.style.height = height + ' !important';
    elm.style.setProperty('height', height, 'important');
}

//水平滑動區塊至消失
function $SlideOut_H(element_or_id) {
    if (!element_or_id) return;
    let elm = $GetElement(element_or_id);
    if (!elm) return;
    //elm.style.width = '0px !important';
    elm.style.setProperty('width', '0px', 'important');
}

//垂直滑動區塊至消失
function $SlideOut_V(element_or_id) {
    if (!element_or_id) return;
    let elm = $GetElement(element_or_id);
    if (!elm) return;
    //elm.style.height = '0px !important';
    elm.style.setProperty('height', '0px', 'important');
}

/*日期、時間函式*/
//將Utc時間轉成本地時間
function UtcToLocalDataTime(date) {
    let m = date.getTimezoneOffset();
    let d = new Date(date);
    d.setMinutes(d.getMinutes() + m);
    return d;
}

//Html編碼
function $HtmlEncode(text) {
    let div = document.createElement('div');
    div.appendChild(document.createTextNode(text));
    return div.textContent || div.innerText;
}
//Html解碼
function $HtmlDecode(text) {
    let div = document.createElement('div');
    div.innerHTML = text;
    return div.textContent || div.innerText;
}

//將物件的屬性併入form的hidden欄位
function AttachObjectToForm(form_or_id, obj) {
    let fm = $GetElement(form_or_id);
    if (!fm) {
        console.log(`form_or_id[${form_or_id}] not found.`);
        return;
    }
    let existingFields = fm.elements; //既有欄位
    if (obj) {
        let data = ConvertToFormData(obj); //分頁參數
        for ([k, v] of data) {
            //若與既有欄位存在相同名稱元素則略過
            if (existingFields[k]) {
                console.log(`the [${k}] property is omitted because there is existing form field with same name`);
                continue;
            } else {
                let hdn = document.createElement('input');
                hdn.setAttribute('type', 'hidden');
                hdn.setAttribute('name', k);
                if (v == null || v == 'null') v = '';
                hdn.setAttribute('value', v);
                fm.appendChild(hdn);
            }
        }
    }
}

//將iframe調整成適配內容大小
function resizeIframeToFitContent(iframe) {

    let padding = 20;
    iframe.style.width = 0;
    iframe.style.height = 0;
    let iframeBody = iframe.contentWindow.document.body;
    iframe.style.width = iframeBody.scrollWidth + padding + 'px';
    iframe.style.height = iframeBody.scrollHeight + padding + 'px';

}

/*Lighter Navbar*/
let $NavbarManager = new $NavbarManagerObject();
function $NavbarManagerObject() {
    this.shownMenus = [];
}
$NavbarManagerObject.prototype.push = function (menu) {
    if (!this.shownMenus.includes(menu)) {
        this.shownMenus.push(menu);
    }
}

$NavbarManagerObject.prototype.clear = function () {
    let menuToBeClose = this.shownMenus.pop();
    while (menuToBeClose) {
        menuToBeClose.hide();
        menuToBeClose = this.shownMenus.pop();
    }
}

function $InitializeNavbar() {
    let navBars = _$('.lighter-navbar', { acceptUndefinedTarget:true });
    if (!navBars || navBars.length == 0) {
        return;
    }

    function toggleSubMenu(toggle,subMenu) {
        if (!subMenu) {
            return;
        }

        let isShown = $NavbarManager.shownMenus.includes(subMenu);
        $NavbarManager.clear();
        if (!isShown) {
            subMenu.show('block');
            $NavbarManager.push(subMenu);
        }
    }

    navBars.forEach((nb) => {
        //navbar-toggle event handling
        let navbarToggle = nb.childElement('.navbar-toggle');
        let navbarBody = nb.childElement('.navbar-body');
        if (navbarToggle) {
            navbarToggle.hide();
            let rwdLevelName = navbarToggle.data('data-rwd-level');
            let rwdLevel = $RWDCutPoint().getRwdByName(rwdLevelName).level;
            if (!rwdLevel || rwdLevel < 0) rwdLevel = 1; //refer to $RWDCutPoint()
            //listen to window rwd change event
            $EventHub().listen('windowRwdChange', (args) => {
                if (args.rwdLevel <= rwdLevel) {
                    if (!navbarBody.containsClass('vertical')) {
                        navbarBody.addClass('vertical');
                    }
                    navbarBody.hide();
                    navbarToggle.show();
                } else {
                    navbarBody.removeClass('vertical');
                    navbarToggle.hide();
                    navbarBody.show();
                }
            });

            //navbar click event handling
            navbarToggle.click(() => {
                if (!navbarBody) {
                    console.log('Warning:navbarToggle.click() aborted due to no .navbar-body element found.');
                    return;
                }
                if (navbarBody.containsClass('hide')) {
                    navbarBody.show();
                } else {
                    navbarBody.hide();
                }
            });
        }

        //sub-menu-toggle event handling
        let subMenuToggles = nb.childElements('.sub-menu-toggle');
        if ((!subMenuToggles) || (subMenuToggles.length == 0)) return;

        subMenuToggles.forEach((toggle) => {
            let subMenu = toggle.sibling('.sub-menu');
            if (!subMenu) {
                console.log('Warning: .sub-menu under toggle element [] not found.');
                return;
            }

            toggle.click((e) => {
                toggleSubMenu(toggle,subMenu);
            });

            toggle.blur((e) => {
                $NavbarManager.clear();
            });

        });
    });


}

/*Lighter DatePicker*/
let $LighterDatePickerStore = [];
function $LighterDatePicker() {
    this.MIN_WIDTH = 300;
    this.DEFAULT_WIDTH = 300;
    this.id = undefined;
    this.year = 0;
    this.month = 0;
    this.day = 0;
    this.value = undefined;
    this.target = undefined;
    this.lang = 'zh-TW';
    this.startDate = new Date('1970-1-1T00:00:00');
    this.endDate = new Date();
    this.startYear = this.startDate.getFullYear();
    this.endYear = this.endDate.getFullYear();
    this.yearOrder = 'asc';
    this.width = 300;
    this.container = undefined;
    this.headerPanel = undefined;
    this.ymPanel = undefined;
    this.wkPanel = undefined;
    this.datePanel = undefined;
    this.cmdPanel = undefined;
    this.yearText = undefined;
    this.yearSelect = undefined;
    this.yearLabelBefore = undefined;
    this.yearLabelAfter = undefined;
    this.monthSelect = undefined;
    this.monthLabelBefore = undefined;
    this.dateItems = [];
    this.previousMonthBtn = undefined;
    this.nextMonthBtn = undefined;
    this.cancelBtn = undefined;
    this.onCompleteCallback = undefined;
    this.zIndex = 3000;

}

$LighterDatePicker.prototype.initialize = function ({id,targetSelector, startDate, endDate, value, lang, yearOrder, width, onCompleteCallback, zIndex } = {}) {

    if (this.container) {
        console.log(`Warning: $LighterDatePicker.initialize() aborted. this object has already been initialized.`)
        return this;
    }

    if (id) {
        this.id = id;
    }

    //langauge
    if (lang) {
        this.lang = lang;
    }

    //start date
    if (startDate) {
        startDate = startDate.replaceAll('/', '-');
        try {
            this.startDate = new Date(startDate);
        } catch (err) {
            console.log(`Warning:$LighterDatePicker.initialze(), the format of startDate argument is invalid`);
        }
    }

    //end date
    if (endDate) {
        endDate = endDate.replaceAll('/', '-');
        try {
            this.endDate = new Date(endDate);
        } catch (err) {
            console.log(`Warning:$LighterDatePicker.initialze(), the format of endtDate argument is invalid`);
        }
    }

    //startYear, endYear
    this.startYear = this.startDate.getFullYear();
    this.endYear = this.endDate.getFullYear();

    //value
    this.setValue(value);

    //year order
    if (yearOrder) {
        yearOrder = yearOrder.toLowerCase();
        if (yearOrder == 'desc' || yearOrder == 'descendant') {
            this.yearOrder = 'desc';
        } else {
            this.yearOrder = 'asc';
        }
    }

    //width
    if (width) {
        if (isNaN(width)) this.width = this.DEFAULT_WIDTH;
        try {
            let intWidth = parseInt(width);
            this.width = Math.max(intWidth, this.MIN_WIDTH);
        } catch (err) {
            console.log(`Warning:$LighterDatePicker.initialze(), the width argument is not a invalid number`);
            this.width = this.DEFAULT_WIDTH;
        }
    }

    //onComplete callback
    if (onCompleteCallback) {
        this.onCompleteCallback = onCompleteCallback;
    }

    //z-index
    if (zIndex) {
        try {
            let intZIndex = parseInt(zIndex);
            if (intZIndex < 3000) intZIndex = 3000; //minium
            this.zIndex = intZIndex;
        } catch (err) {
            //safely ignore this exception
        }
    }

    this.createUiComponent();
    this.setYearMonth(this.year, this.month);
    this.refreshDateItems(this.year, this.month);

    let thisPicker = this;

    //binding to target elements
    this.bind(targetSelector);

    _$(this.dateItems).click((e) => {
        e.stopPropagation();
        let selected = e.currentTarget;
        let year = selected.getAttribute('data-year');
        let month = selected.getAttribute('data-month');
        let day = selected.getAttribute('data-day');
        thisPicker.value = new Date(`${year}-${month}-${day}`);

        let langModeIndex = getLangModeIndex(thisPicker.lang);
        if (langModeIndex == 0) {
            //民國年
            thisPicker.target.value = `${year - 1911}-${month}-${day}`;
        } else {
            //西元年
            thisPicker.target.value = `${year}-${month}-${day}`;
        }

        thisPicker.hide();
        thisPicker.target.focus();
        if (thisPicker.onCompleteCallback) {
            thisPicker.onCompleteCallback({
                target: thisPicker.target,
                value: thisPicker.value,
                year: year,
                month: month,
                day: day
            });
        }
    });

    let $yearText = _$(this.yearText);
    $yearText.click((e) => {
        e.stopPropagation();
    });

    $yearText.change((e) => {
        e.stopPropagation();
        let value = $yearText.value();
        let langModeIndex = getLangModeIndex(thisPicker.lang);
        if (!value) {
            //e.preventDefault();
            if (langModeIndex == 0) {
                //民國年
                $PopMessage({ message: '年份不可為空值或0，請重新輸入' });
            } else {
                //西元年
                $PopMessage({ message: 'The year value must not be empty or zero, please enter again' });
            }

            return;
        }

        let year = 0;

        try {
            year = parseInt(value);
        } catch (err) {
            if (langModeIndex == 0) {
                //民國年
                $PopMessage({ message: '年份格式必須是數字，請檢查' });
            } else {
                //西元年
                $PopMessage({ message: 'The year value must not be number, please check again' });
            }
            return;
        }

        let normalizedYear = year;
        if (langModeIndex == 0) {
            //民國年轉西元年
            normalizedYear = year + 1911;
        }

        if (normalizedYear < thisPicker.startYear || normalizedYear > thisPicker.endYear) {
            if (langModeIndex == 0) {
                //民國年
                $PopMessage({ message: `年份範圍必須介於${thisPicker.startYear-1911}~${thisPicker.endYear-1911}之間，請檢查` });
            } else {
                //西元年
                $PopMessage({ message: `The year value must be between ${thisPicker.startYear}~${thisPicker.endYear}, please check again` });
            }
            return;
        }

        thisPicker.year = normalizedYear;

        thisPicker.setYearMonth(thisPicker.year, thisPicker.month);
        thisPicker.refreshDateItems(thisPicker.year, thisPicker.month);
    });

    let $monthSelect = _$(this.monthSelect);
    $monthSelect.click((e) => {
        e.stopPropagation();
    });
    $monthSelect.change((e) => {
        e.stopPropagation();
        thisPicker.month = parseInt($monthSelect.value());
        thisPicker.setYearMonth(thisPicker.year, thisPicker.month);
        thisPicker.refreshDateItems(thisPicker.year, thisPicker.month);
    });

    _$(this.cancelBtn).click((e) => {
        e.stopPropagation();
        thisPicker.hide();
    });

    _$(this.previousMonthBtn).click((e) => {
        e.stopPropagation();
        let year = this.year;
        let month = this.month - 1;
        if (month == 0) {
            month = 12;
            year--;
        }
        thisPicker.setYearMonth(year, month);
        thisPicker.refreshDateItems(year, month);
    });

    _$(this.nextMonthBtn).click((e) => {
        e.stopPropagation();
        let year = this.year;
        let month = this.month + 1;
        if (month == 13) {
            month = 1;
            year++;
        }
        thisPicker.setYearMonth(year, month);
        thisPicker.refreshDateItems(year, month);
    });

    _$(this.container).click((e) => {
        e.stopPropagation();
    });

    document.body.addEventListener('click', () => {
        thisPicker.hide();
    });

    $LighterDatePickerStore.push(this);

    console.log(`datepicker(id=${this.id}) initialized`);

    return this;
}

$LighterDatePicker.prototype.bind = function (targetSelector) {
    if (!targetSelector) {
        console.log(`Error: $LighterDatePicker.bind() failed. the targetSelector argument is missing or undefined.`);
        return;
    }

    //binding targets and event handling
    let targets = _$(targetSelector);
    if (!targets.valid) {
        console.log(`Error: $LighterDatePicker.bind() failed. the targetSelector is not a valid selector, element or no dom element matches the selector.`);
        return;
    }

    let thisPicker = this;
    targets.click((e) => {
        e.stopPropagation();
        thisPicker.target = e.currentTarget;
        let value = thisPicker.target.value;
        let langModeIndex = getLangModeIndex(thisPicker.lang);
        let dateParts = [];
        if (value) {
            if (value.indexOf('/') >= 0) {
                dateParts = value.split('/');
            } else if (value.indexOf('-') >= 0) {
                dateParts = value.split('-');
            }
        }
        if (langModeIndex == 0) {
            //民國年轉西元年
            if (value) {
                if (dateParts.length > 0) {
                    try {
                        dateParts[0] = parseInt(dateParts[0]) + 1911;
                        value = dateParts.join('-');
                    } catch (err) {
                        //safely ignore this error
                    }
                }
            }
        }

        if (dateParts.length > 1) {
            thisPicker.setValue(value);
        } else {
            thisPicker.setValue('');
        }

        thisPicker.setYearMonth(thisPicker.year, thisPicker.month);
        thisPicker.refreshDateItems(thisPicker.year, thisPicker.month);
        for (const dp of $LighterDatePickerStore) {
            if (dp.id != thisPicker.id) {
                dp.hide();
            }
        }
        thisPicker.show();

    });

    targets.keyup((e) => {
        e.stopPropagation();
        if (e.isComposing || e.keyCode === 229) {
            return;
        }

        if (e.code == 'Enter' || e.code == 'Tab' || e.code == 'Escape') {
            thisPicker.hide();
        }
    });

}

$LighterDatePicker.prototype.setValue = function(value) {
    if (value) {
        value = value.replaceAll('/', '-');
        try {
            this.value = new Date(value);
        } catch (err) {
            console.log(`Warning:$LighterDatePicker.setValue() aborted, the format of value argument:${value} is not a valid date`);
            this.value = new Date();
        }
    } else {
        this.value = new Date();
    }

    //year, month, day
    this.year = this.value.getFullYear();
    this.month = this.value.getMonth() + 1;
    this.day = this.value.getDate();
}

$LighterDatePicker.prototype.getPreviousYearMonth = function (year, month) {
    if (arguments.length < 2) {
        year = this.year;
        month = this.month;
    }
    month --;
    if (month == 0) {
        month = 12;
        year--;
    }

    return {year:year, month:month};
}

$LighterDatePicker.prototype.getNextYearMonth = function (year, month) {
    if (arguments.length < 2) {
        year = this.year;
        month = this.month;
    }

    month ++;
    if (month == 13) {
        month = 1;
        year++;
    }

    return { year: year, month: month };
}

$LighterDatePicker.prototype.show = function () {
    let rect = $ElementRect(this.target);
    this.container.style.setProperty('top', `${rect.pageTop + rect.height}px`, 'important');
    this.container.style.setProperty('left', `${rect.pageLeft}px`, 'important');
    this.container.classList.remove('hide');
    this.container.scrollIntoView({block:'end'});
}

$LighterDatePicker.prototype.hide = function () {
    if (!this.container.classList.contains('hide')) {
        this.container.classList.add('hide');
    }
}

function getLangModeIndex(lang) {
    let index = 0;
    switch (lang) {
        case 'zh-TW':
        case 'zh-CN':
        case 'zh-Hant-TW':
        case 'zh-Hant':
        case 'zh-Hans-CN':
        case 'zh-Hans':
        default:
            index = 0;
            break;
        case 'en':
            index = 1;
            break;
    }
    return index;
}

function getWeekdayItemArr(langModeIndex) {
    let weekdayItemGroups = [
        ['日', '一', '二', '三', '四', '五', '六'],
        ['SUN', 'MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT']
    ];
    if (!langModeIndex || langModeIndex > weekdayItemGroups.length - 1) langModeIndex = 0;
    return weekdayItemGroups[langModeIndex];
}

function getMonthItemArr(langModeIndex) {
    let monthItemGroups = [
        ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        ['Jan.', 'Feb.', 'Mar.', 'Apr.', 'May', 'Jun.', 'Jul.', 'Aug.', 'Sep.', 'Oct.', 'Nov.', 'Dec.']
    ];
    if (!langModeIndex || langModeIndex > monthItemGroups.length - 1) langModeIndex = 0;
    return monthItemGroups[langModeIndex];
}

$LighterDatePicker.prototype.createUiComponent = function () {
    let y = this.value.getFullYear();
    let m = this.value.getMonth() + 1;
    let d = this.value.getDate();
    let wd = this.value.getDay;
    this.container = document.createElement('div');
    this.headerPanel = document.createElement('div');
    this.ymPanel = document.createElement('div');
    this.wkPanel = document.createElement('div');
    this.datePanel = document.createElement('div');
    this.cmdPanel = document.createElement('div');

    //create command buttons
    this.cancelBtn = document.createElement('button');
    this.previousMonthBtn = document.createElement('button');
    this.nextMonthBtn = document.createElement('button');
    this.cancelBtn.setAttribute('type','button');
    this.previousMonthBtn.setAttribute('type','button');
    this.nextMonthBtn.setAttribute('type','button');

    //style
    this.container.style = `width:${this.width}px !important;min-width:${this.width}px !important;max-width:${this.width}px !important;`;
    this.container.classList.add('lighter-datepicker', 'hide', 'test-datepicker');
    if (this.zIndex > 3000) {
        this.container.style.setProperty('z-index', this.zIndex,'important');
    }
    this.headerPanel.classList.add('header-panel');
    this.ymPanel.classList.add('year-month-panel');
    this.wkPanel.classList.add('weekday-panel');
    this.datePanel.classList.add('date-panel');

    this.cancelBtn.classList.add('btn-square-xxl', 'icon', 'close-icon');
    this.previousMonthBtn.classList.add('btn-square-xxl', 'icon', 'previous-icon');
    this.nextMonthBtn.classList.add('btn-square-xxl', 'icon', 'next-icon');

    let langModeIndex = getLangModeIndex(this.lang);

    //create headerPanel
    //create ymPanel
    //year options
    //this.yearSelect = document.createElement('select');
    this.yearText = document.createElement('input');
    this.yearText.setAttribute('type', 'number');
   
    if (this.startYear) {
        if (langModeIndex == 0) {
            //民國年
            this.yearText.setAttribute('min', this.startYear - 1911);
        } else {
            //西元年
            this.yearText.setAttribute('min', this.startYear);
        }
    }
    if (this.endYear) {
        if (langModeIndex == 0) {
            //民國年
            this.yearText.setAttribute('max', this.endYear - 1911);
        } else {
            //西元年
            this.yearText.setAttribute('max', this.endYear);
        }
    }

    this.yearLabelBefore = document.createElement('span');
    this.yearLabelAfter = document.createElement('span');
    switch (langModeIndex) {
        case 0:
        default:
            this.yearLabelBefore.innerText = '民國年';
            this.yearLabelAfter.innerText = ' ';
            break;
        case 1:
            this.yearLabelBefore.innerText = 'Year';
            this.yearLabelAfter.innerText = ' ';
            break;
    }

    this.container.appendChild(this.headerPanel);
    this.headerPanel.appendChild(this.ymPanel);
    this.headerPanel.appendChild(this.previousMonthBtn);
    this.headerPanel.appendChild(this.nextMonthBtn);
    this.headerPanel.appendChild(this.cancelBtn);
    this.ymPanel.appendChild(this.yearLabelBefore);
    this.ymPanel.appendChild(this.yearText);
    this.ymPanel.appendChild(this.yearLabelAfter);

    //month options
    this.monthSelect = document.createElement('select');
    this.monthLabelBefore = document.createElement('span');
    let monthItemArr = getMonthItemArr(langModeIndex);
    for (let i = 1; i <= 12; i++) {
        let op = document.createElement('option');
        op.value = i;
        op.text = `${monthItemArr[i - 1]}`;
        switch (langModeIndex) {
            case 0:
            default:
                this.monthLabelBefore.innerText = '月份';
                break;
            case 1:
                this.monthLabelBefore.innerText = 'Month';
                break;
        }
        if (i == m) {
            op.selected = true;
        }
        this.monthSelect.appendChild(op);

    }

    this.ymPanel.appendChild(this.monthLabelBefore);
    this.ymPanel.appendChild(this.monthSelect);

    //create wkPanel
    let weekdayItemArr = getWeekdayItemArr(langModeIndex);

    for (let i = 0; i < weekdayItemArr.length; i++) {
        let weekDayItem = document.createElement('b');
        weekDayItem.innerText = weekdayItemArr[i];
        this.wkPanel.appendChild(weekDayItem);
    }
    this.container.appendChild(this.wkPanel);

    //create dayPanel
    let dateItemCount = 7 * 6;
    for (let i = 0; i < dateItemCount; i++) {
        let dateItem = document.createElement('b');
        dateItem.classList.add('date-item');
        dateItem.innerText = i;
        this.datePanel.appendChild(dateItem);
        this.dateItems.push(dateItem);
    }

    this.container.appendChild(this.datePanel);
    document.body.appendChild(this.container);

}

$LighterDatePicker.prototype.setYearMonth = function (year, month) {
    if (year < this.startYear || year > this.endYear) {
        console.log(`Warning: $LighterDatePicker.setYearMonth() aborted, the year argument:${year} is out of range[${this.startYear}..${this.endYear}]`);
        return;
    }
    if (month < 1 || month > 12) {
        console.log(`Warning: $LighterDatePicker.setYearMonth() aborted, the month argument:${month} is out of range[${1}..${12}]`);
        return;
    }

    this.year = year;
    this.month = month;
    let langModeIndex = getLangModeIndex(this.lang);
    if (langModeIndex == 0) {
        //民國年
        this.yearText.value = year - 1911;
    } else {
        //西元年
        this.yearText.value = year;
    }
   

    SetValue(this.monthSelect, month);

    if (this.year == this.startYear && this.month == this.startDate.getMonth() + 1) {
        _$(this.previousMonthBtn).disable();
    } else {
        _$(this.previousMonthBtn).enable();
    }

    if (this.year == this.endYear && this.month == this.endDate.getMonth() + 1) {
        _$(this.nextMonthBtn).disable();
    } else {
        _$(this.nextMonthBtn).enable();
    }
}

$LighterDatePicker.prototype.refreshDateItems = function (year, month) {
    let yStart = this.startDate.getFullYear();
    let yEnd = this.endDate.getFullYear();
    if (year < yStart || year > yEnd) {
        console.log(`Warning: $LighterDatePicker.setYearMonth() aborted, the year argument:${year} is out of range[${yStart}..${yEnd}]`);
        return;
    }
    //if (year < 1970) {
    //    console.log(`Warning: $LighterDatePicker.refreshDateItems() aborted, the year argument:${year} is less than the minimum limit 1970.`);
    //    return;
    //}
    if (month < 1 || month > 12) {
        console.log(`Warning: $LighterDatePicker.setYearMonth() aborted, the month argument:${month} is out of range[${1}..${12}]`);
        return;
    }
    let monthDayCounts = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    //check 閏年/leap year
    let isLeap = ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0) || (year % 1000 == 0);
    if (month == 2 && isLeap) monthDayCounts[1] = 29;

    let firstDate = new Date(year, month-1, 1);
    let thisMonthDayCount = monthDayCounts[month - 1];
    let preMonthDayCount = 0;
    if (month == 1) {
        preMonthDayCount = 31;
    } else if (month == 12) {
        preMonthDayCount = 30;
    } else {
        preMonthDayCount = monthDayCounts[month - 2];
    }

    let firstIndex = firstDate.getDay();
    let count = 0;

    //previous month
    if (firstIndex > 0) {
        let preYM = this.getPreviousYearMonth(year,month);
        for (let i = 0; i < firstIndex; i++) {
            let day = preMonthDayCount - i
            this.dateItems[count].innerText = day;
            this.dateItems[count].setAttribute('data-year', preYM.year);
            this.dateItems[count].setAttribute('data-month', preYM.month);
            this.dateItems[count].setAttribute('data-day', day);
            this.dateItems[count].classList.remove('selected');
            if (!this.dateItems[count].classList.contains('other-month')) {
                this.dateItems[count].classList.add('other-month');
            }
            count++;
        }
    }

    //current month
    let valueYear = this.value.getFullYear();
    let valueMonth = this.value.getMonth() + 1;
    for (let i = 1; i <= thisMonthDayCount; i++) {
        this.dateItems[count].innerText = i;
        this.dateItems[count].setAttribute('data-year', year);
        this.dateItems[count].setAttribute('data-month', month);
        this.dateItems[count].setAttribute('data-day', i);
        this.dateItems[count].classList.remove('selected');

        if (this.dateItems[count].classList.contains('other-month')) {
            this.dateItems[count].classList.remove('other-month');
        }
        if (count > 28) {
            this.dateItems[count].classList.remove('hide');
        }

        if (year == valueYear && month == valueMonth && this.day == i) {
            this.dateItems[count].classList.add('selected');
        }

        count++;
    }

    //next month
    let remaining = count % 7;
    if (remaining > 0) {
        let nextYM = this.getNextYearMonth(year,month);
        for (let i = 1; i <= 7 - remaining; i++) {
            this.dateItems[count].innerText = i;
            this.dateItems[count].setAttribute('data-year', nextYM.year);
            this.dateItems[count].setAttribute('data-month', nextYM.month);
            this.dateItems[count].setAttribute('data-day', i);
            this.dateItems[count].classList.remove('selected');

            if (!this.dateItems[count].classList.contains('other-month')) {
                this.dateItems[count].classList.add('other-month');
            }

            if (this.dateItems[count].classList.contains('hide')) {
                this.dateItems[count].classList.remove('hide');
            }

            count++;
        }
    }

    //hide the extra dateItems
    let itemLength = 7 * 6;
    if (count < itemLength) {
        for (let i = count; i < itemLength; i++) {
            if (!this.dateItems[i].classList.contains('hide')) {
                this.dateItems[i].classList.add('hide');
            }
        }
    }
}

/*Lighter Tab*/
function $TabNavObject(navContainer, navItems) {
    if (!navContainer) {
        console.log('$TabNavObject() failed. navContainer is undefined.');
    }
    if (!navItems) navItems = [];
    this.navContainer = navContainer;
    this.navItems = [...navItems];
    this.tabGroup = undefined;
}

// function $TabGroupObject(groupContainer,tabItems){
//     if(!groupContainer){
//         console.log('$TabGroupObject() failed. groupContainer is undefined.');
//     }
//     if(!tabItems) tabItems = [];
//     this.groupContainer=groupContainer;
//     this.tabItems=[...tabItems];
// }
// function $TabObject(tabNav,tabGroup){
//     this.tabNav=tabNav;
//     this.tabGroup=tabGroup;
// }

//Tab輔助函式物件
function $TabUtilObject() { }
//Tab初始化(事件和行為)
$TabUtilObject.prototype.initialize = function (tabNavSelector) {
    if (!tabNavSelector) tabNavSelector = '.tab-nav';
    let found = document.querySelectorAll(tabNavSelector);
    if (!found) return;
    let tabNavs = [...found];
    let tabName = '';
    let activeTabName = '';
    for (const nav of tabNavs) {
        let navObj = new $TabNavObject(
            nav,
            nav.querySelectorAll('.nav-item')
        );

        let tabGroupName = nav.getAttribute('data-tab-group-target');
        if (!tabGroupName) {
            console.log(`the data-tab-group-target attribute is not set for tab-nav: $(nav)`);
        }
        let tabGroup = document.querySelector('[data-tab-group-name="tab_group_1"]');
        if (!tabGroup) {
            console.log('[data-tab-group-name="tab_group_1"] not found.');
            continue;
        }

        navObj.tabGroup = tabGroup;
        let tabItems = tabGroup.querySelectorAll('.tab-item');
        if (!tabItems) {
            console.log(`the tabGroup with data-tab-group-name="${tabGroupName}" does not contain any tab-item`);
            continue;
        } else {
            tabItems = [...tabItems];
        }

        for (const ni of navObj.navItems) {
            let tabTargetName = ni.getAttribute('data-tab-target');
            if (!tabTargetName) {
                console.log(`data-tab-target is not set for ${ni}`);
                continue;
            }

            let tabTarget = tabItems.find(x => x.getAttribute('data-tab-name') == tabTargetName);
            if (!tabTarget) {
                console.log(`tabTarget not found for tab-items with data-tab-name="${tabTargetName}"`);
                continue;
            }

            //設定tab可視性
            for (const tab of tabItems) {
                if (tab.classList.contains('active')) {
                    Show(tab);
                } else {
                    Hide(tab);
                }
            }


            //綁定click事件
            let trigger = ni.querySelector('[data-tab-target]');
            if (!trigger) trigger = ni;
            trigger.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                activeTabName = trigger.getAttribute('data-tab-target');

                for (const ni2 of navObj.navItems) {
                    ni2.classList.remove('active');
                }
                ni.classList.add('active');

                //隱藏全部tab
                for (const tab of tabItems) {
                    tabName = tab.getAttribute('data-tab-name');
                    if (tabName == activeTabName) {
                        //顯示
                        tab.classList.remove('hide');
                        tab.classList.add('active');
                    } else {
                        //隱藏
                        tab.classList.add('hide');
                        tab.classList.remove('active');
                    }
                }
            });
        }
    }
}

//Tab輔助函式
function $TabUtil() {
    return new $TabUtilObject();
}

//JS Map輔助函式
let $MapStore = new Map();
function $CreateMap(listJson,keyFieldName, mapId ) {
    let list = ConvertToJSObject(listJson);
    if (!list) {
        console.log(`$CreateMap() failed. listJson argument is not a valid json`);
        return;
    }

    if (!Array.isArray(list)) {
        console.log(`$CreateMap() failed. listJson argument is not a valid json array`);
        return;
    }

    let map = new Map();
    
    for (let item of list) {
        let key = item[keyFieldName];
        if (!key) {
            console.log(`$CreateMap() failed. keyFieldName=${keyFieldName} is not a valid field name of the item of listJson argument`);
            return;
        }

        let isDuplicateKey = map.get(key);
        if (isDuplicateKey) {
            console.log(`$CreateMap() failed. key=${key} is duplicated`);
            return;
        }

        map.set(key, item);
    }

    if (mapId) {
        if ($MapStore.get(mapId)) {
            console.log(`warning: $CreateMap() the given mapId(=${mapId}) argument already exsited.`);
            return;
        }

        $MapStore.set(mapId,map);
    }

    return map;
}

function $Map(mapId) {
    if (!mapId) {
        console.log(`$Map() failed. Missing mapId argument.`);
        return undefined;
    }

    let map = $MapStore.get(mapId);
    if (!map) {
        console.log(`$Map() failed. mapId=${mapId} not found.`);
        return undefined;
    }
    return map;
}

/***檔案輔助函式***/
function $FormatFilesize(fileLengthInByte, decimal_digits) {
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


/***轉換輔助函式***/
function $Int(value, defaultValue) {
    if (value === undefined || value == '' || isNaN(value)) {
        if (defaultValue !== undefined) {
            return defaultValue;
        } else {
            console.log(`warning: getData(), 0 is returned due to defaultValue is undefined`);
            return 0;
        }
    } else {
        return parseInt(value);
    }
}

/***Array Utils***/
function $ArrayExcept(arr1, arr2) {
    if (!arr1 || arr1.length == 0) return [];
    if (!arr2 || arr2.len == 0) return arr1.map(x=>x);
    var filtered = arr1.filter(x => !arr2.includes(x));
    return filtered;
}
