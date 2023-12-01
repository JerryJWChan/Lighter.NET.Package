/*This javascript library is to provide frequently used icons in svg format 
 * To prevent duplicated svg element id, include this file on the top of the page and do not include this file twice.
 * 此js提供常用的svg icon, 增加svg定義時注意避免id重複，並且只可以include此檔案一次。
 */


//svg icon 集合
let svgIconCollection = [];
/*之後將全部svg icon定義push到 svgIconCollection中 */
/*每個svg icon定義必須有唯一的id屬性 */

//核取方塊(未勾選)
svgIconCollection.push(`<svg
   width="16"
   height="16"
   version="1.1"
   xml:space="preserve"
   inkscape:version="1.2 (dc2aedaf03, 2022-05-15)"
   sodipodi:docname="square_24x24.svg"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:svg="http://www.w3.org/2000/svg"><sodipodi:namedview
     id="namedview7"
     pagecolor="#ffffff"
     bordercolor="#000000"
     borderopacity="0.25"
     inkscape:showpageshadow="2"
     inkscape:pageopacity="0.0"
     inkscape:pagecheckerboard="0"
     inkscape:deskcolor="#d1d1d1"
     inkscape:document-units="mm"
     showgrid="false"
     inkscape:zoom="2.1089995"
     inkscape:cx="47.890007"
     inkscape:cy="52.157433"
     inkscape:window-width="2560"
     inkscape:window-height="1017"
     inkscape:window-x="-8"
     inkscape:window-y="-8"
     inkscape:window-maximized="1"
     inkscape:current-layer="layer1" /><defs
     id="defs_square_24x24">
    <symbol 
        id="svg_icon_square_24x24" 
        width="24"
        height="24"
        viewBox="0 0 6.3499999 6.35">
    <g
     inkscape:label="圖層 1"
     inkscape:groupmode="layer"
     id="layer1"><path
       id="rect374"
       style="color:#000000;fill:#000000;-inkscape-stroke:none"
       d="M 0.0857829,-0.0392741 V 0.234611 6.31073 H 6.43578 V -0.0392741 Z M 0.634587,0.507979 H 5.88853 V 5.76347 H 0.634587 Z" /></g>
</symbol>
</defs>
</svg>`);

//核取方塊(勾選)
svgIconCollection.push(`<svg
   width="16"
   height="16"
   version="1.1"
   xml:space="preserve"
   inkscape:version="1.2 (dc2aedaf03, 2022-05-15)"
   sodipodi:docname="square_checked_24x24.svg"
   inkscape:export-filename="square_checked_24x24_plain.svg"
   inkscape:export-xdpi="96"
   inkscape:export-ydpi="96"
   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
   xmlns="http://www.w3.org/2000/svg"
   xmlns:svg="http://www.w3.org/2000/svg"><sodipodi:namedview
     id="namedview7"
     pagecolor="#ffffff"
     bordercolor="#000000"
     borderopacity="0.25"
     inkscape:showpageshadow="2"
     inkscape:pageopacity="0.0"
     inkscape:pagecheckerboard="0"
     inkscape:deskcolor="#d1d1d1"
     inkscape:document-units="mm"
     showgrid="false"
     inkscape:zoom="4.2179991"
     inkscape:cx="37.22144"
     inkscape:cy="37.695599"
     inkscape:window-width="2560"
     inkscape:window-height="1017"
     inkscape:window-x="-8"
     inkscape:window-y="-8"
     inkscape:window-maximized="1"
     inkscape:current-layer="layer1" /><defs
     id="defs_square_checked_24x24">
    <symbol 
        id="svg_icon_square_checked_24x24"
        width="24"
        height="24"
        viewBox="0 0 6.35 6.35">
    <g
     inkscape:label="圖層 1"
     inkscape:groupmode="layer"
     id="layer1"
     style="display:inline"><path
       id="rect374"
       style=""
       d="M 0.0857829,-0.0392741 V 0.234611 6.31073 H 6.43578 V -0.0392741 Z M 0.634587,0.507979 H 5.88853 V 5.76347 H 0.634587 Z M 4.99194,0.806669 2.70681,4.00596 1.48415,2.88458 1.02526,3.38687 2.81275,5.0276 5.54488,1.20096 Z" /></g>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)圓圈箭頭向右
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
class="bi bi-arrow-right-circle">
  <defs>
    <symbol
        id="svg_icon_circle_arrow_right" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path fill-rule="evenodd" d="M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM4.5 7.5a.5.5 0 0 0 0 1h5.793l-2.147 2.146a.5.5 0 0 0 .708.708l3-3a.5.5 0 0 0 0-.708l-3-3a.5.5 0 1 0-.708.708L10.293 7.5H4.5z"/>
  </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)打勾
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_check_bold" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M12.736 3.97a.733.733 0 0 1 1.047 0c.286.289.29.756.01 1.05L7.88 12.01a.733.733 0 0 1-1.065.02L3.217 8.384a.757.757 0 0 1 0-1.06.733.733 0 0 1 1.047 0l3.052 3.093 5.4-6.425a.247.247 0 0 1 .02-.022Z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)打叉 X
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_x" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)加號 +
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_plus" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path fill-rule="evenodd" d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2Z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)編輯(鉛筆) 
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_edit" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z"/>
        <path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z"/>        
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)QR-Code
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_qrcode" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M2 2h2v2H2V2Z"/>
        <path d="M6 0v6H0V0h6ZM5 1H1v4h4V1ZM4 12H2v2h2v-2Z"/>
        <path d="M6 10v6H0v-6h6Zm-5 1v4h4v-4H1Zm11-9h2v2h-2V2Z"/>
        <path d="M10 0v6h6V0h-6Zm5 1v4h-4V1h4ZM8 1V0h1v2H8v2H7V1h1Zm0 5V4h1v2H8ZM6 8V7h1V6h1v2h1V7h5v1h-4v1H7V8H6Zm0 0v1H2V8H1v1H0V7h3v1h3Zm10 1h-1V7h1v2Zm-1 0h-1v2h2v-1h-1V9Zm-4 0h2v1h-1v1h-1V9Zm2 3v-1h-1v1h-1v1H9v1h3v-2h1Zm0 0h3v1h-2v1h-1v-2Zm-4-1v1h1v-2H7v1h2Z"/>
        <path d="M7 12h1v3h4v1H7v-4Zm9 2v2h-3v-1h2v-1h1Z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)查詢(放大鏡)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_search" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z" />
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)回頁首(圓圈箭頭向上)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_go_top" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path fill-rule="evenodd" d="M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-7.5 3.5a.5.5 0 0 1-1 0V5.707L5.354 7.854a.5.5 0 1 1-.708-.708l3-3a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1-.708.708L8.5 5.707V11.5z" />
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)訊息提示(圓圈驚嘆號)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_remind" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
        <path d="M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 4.995z"/>        
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)人員(個人資訊卡)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_user_card" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M5 8a2 2 0 1 0 0-4 2 2 0 0 0 0 4Zm4-2.5a.5.5 0 0 1 .5-.5h4a.5.5 0 0 1 0 1h-4a.5.5 0 0 1-.5-.5ZM9 8a.5.5 0 0 1 .5-.5h4a.5.5 0 0 1 0 1h-4A.5.5 0 0 1 9 8Zm1 2.5a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1-.5-.5Z"/>
        <path d="M2 2a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V4a2 2 0 0 0-2-2H2ZM1 4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v8a1 1 0 0 1-1 1H8.96c.026-.163.04-.33.04-.5C9 10.567 7.21 9 5 9c-2.086 0-3.8 1.398-3.984 3.181A1.006 1.006 0 0 1 1 12V4Z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)資訊(方塊i)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_info_square" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/>
        <path d="m8.93 6.588-2.29.287-.082.38.45.083c.294.07.352.176.288.469l-.738 3.468c-.194.897.105 1.319.808 1.319.545 0 1.178-.252 1.465-.598l.088-.416c-.2.176-.492.246-.686.246-.275 0-.375-.193-.304-.533L8.93 6.588zM9 4.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)提示(圓形驚嘆號)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_exclame_circle" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
            <path d="M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 4.995z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)前往連結(方塊箭頭右上)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_link_out_up_right" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M8.636 3.5a.5.5 0 0 0-.5-.5H1.5A1.5 1.5 0 0 0 0 4.5v10A1.5 1.5 0 0 0 1.5 16h10a1.5 1.5 0 0 0 1.5-1.5V7.864a.5.5 0 0 0-1 0V14.5a.5.5 0 0 1-.5.5h-10a.5.5 0 0 1-.5-.5v-10a.5.5 0 0 1 .5-.5h6.636a.5.5 0 0 0 .5-.5z"/>
            <path fill-rule="evenodd" d="M16 .5a.5.5 0 0 0-.5-.5h-5a.5.5 0 0 0 0 1h3.793L6.146 9.146a.5.5 0 1 0 .708.708L15 1.707V5.5a.5.5 0 0 0 1 0v-5z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)付款(信用卡)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_credit_card" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path d="M11 5.5a.5.5 0 0 1 .5-.5h2a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1-.5-.5v-1z"/>
            <path d="M2 2a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V4a2 2 0 0 0-2-2H2zm13 2v5H1V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1zm-1 9H2a1 1 0 0 1-1-1v-1h14v1a1 1 0 0 1-1 1z"/>
    </symbol>
    </defs>
</svg>`);

//(bootstrap-icon)上傳(方塊折角+箭頭向上)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_upload_file_arrow_up" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path d="M8.5 11.5a.5.5 0 0 1-1 0V7.707L6.354 8.854a.5.5 0 1 1-.708-.708l2-2a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 7.707V11.5z"/>
            <path d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z"/>    
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)上傳(容器+箭頭向上)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_upload_arrow_up" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
            <path d="M7.646 1.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1-.708.708L8.5 2.707V11.5a.5.5 0 0 1-1 0V2.707L5.354 4.854a.5.5 0 1 1-.708-.708l3-3z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)下載(容器+箭頭向下)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_download_arrow_down" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
            <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)項目清單(數字前綴)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_order_list" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5z"/>
        <path d="M1.713 11.865v-.474H2c.217 0 .363-.137.363-.317 0-.185-.158-.31-.361-.31-.223 0-.367.152-.373.31h-.59c.016-.467.373-.787.986-.787.588-.002.954.291.957.703a.595.595 0 0 1-.492.594v.033a.615.615 0 0 1 .569.631c.003.533-.502.8-1.051.8-.656 0-1-.37-1.008-.794h.582c.008.178.186.306.422.309.254 0 .424-.145.422-.35-.002-.195-.155-.348-.414-.348h-.3zm-.004-4.699h-.604v-.035c0-.408.295-.844.958-.844.583 0 .96.326.96.756 0 .389-.257.617-.476.848l-.537.572v.03h1.054V9H1.143v-.395l.957-.99c.138-.142.293-.304.293-.508 0-.18-.147-.32-.342-.32a.33.33 0 0 0-.342.338v.041zM2.564 5h-.635V2.924h-.031l-.598.42v-.567l.629-.443h.635V5z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)項目清單(圓點前綴)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_unorder_list" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M5 11.5a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h9a.5.5 0 0 1 0 1h-9a.5.5 0 0 1-.5-.5zm-3 1a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2zm0 4a1 1 0 1 0 0-2 1 1 0 0 0 0 2z"/>        
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)項目清單(階層)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_nest_list" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
            <path fill-rule="evenodd" d="M4.5 11.5A.5.5 0 0 1 5 11h10a.5.5 0 0 1 0 1H5a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 3 7h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm-2-4A.5.5 0 0 1 1 3h10a.5.5 0 0 1 0 1H1a.5.5 0 0 1-.5-.5z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)文字(折角文件)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_document_earmark" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M5.5 7a.5.5 0 0 0 0 1h5a.5.5 0 0 0 0-1h-5zM5 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm0 2a.5.5 0 0 1 .5-.5h2a.5.5 0 0 1 0 1h-2a.5.5 0 0 1-.5-.5z"/>
        <path d="M9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V4.5L9.5 0zm0 1v2A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)User(單人)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_person" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6Zm2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0Zm4 8c0 1-1 1-1 1H3s-1 0-1-1 1-4 6-4 6 3 6 4Zm-1-.004c-.001-.246-.154-.986-.832-1.664C11.516 10.68 10.289 10 8 10c-2.29 0-3.516.68-4.168 1.332-.678.678-.83 1.418-.832 1.664h10Z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)User(單人填滿)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_person_fill" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3Zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)User(雙人)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_person_group" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M15 14s1 0 1-1-1-4-5-4-5 3-5 4 1 1 1 1h8Zm-7.978-1A.261.261 0 0 1 7 12.996c.001-.264.167-1.03.76-1.72C8.312 10.629 9.282 10 11 10c1.717 0 2.687.63 3.24 1.276.593.69.758 1.457.76 1.72l-.008.002a.274.274 0 0 1-.014.002H7.022ZM11 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4Zm3-2a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM6.936 9.28a5.88 5.88 0 0 0-1.23-.247A7.35 7.35 0 0 0 5 9c-4 0-5 3-5 4 0 .667.333 1 1 1h4.216A2.238 2.238 0 0 1 5 13c0-1.01.377-2.042 1.09-2.904.243-.294.526-.569.846-.816ZM4.92 10A5.493 5.493 0 0 0 4 13H1c0-.26.164-1.03.76-1.724.545-.636 1.492-1.256 3.16-1.275ZM1.5 5.5a3 3 0 1 1 6 0 3 3 0 0 1-6 0Zm3-2a2 2 0 1 0 0 4 2 2 0 0 0 0-4Z"/>
    </symbol>
  </defs>
</svg>`);

//(bootstrap-icon)User(雙人填滿)
svgIconCollection.push(`<svg 
xmlns="http://www.w3.org/2000/svg" 
width="16" 
height="16" 
fill="currentColor" 
viewBox="0 0 16 16"
>
  <defs>
    <symbol
        id="svg_icon_person_group_fill" 
        width="16" 
        height="16" 
        viewBox="0 0 16 16">
        <path d="M7 14s-1 0-1-1 1-4 5-4 5 3 5 4-1 1-1 1H7Zm4-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6Zm-5.784 6A2.238 2.238 0 0 1 5 13c0-1.355.68-2.75 1.936-3.72A6.325 6.325 0 0 0 5 9c-4 0-5 3-5 4s1 1 1 1h4.216ZM4.5 8a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5Z"/>
    </symbol>
  </defs>
</svg>`);

/*於頁面載入時(注意：此段程式碼不可放在window.onload事件中)，動態產生一次svg tags，之後面頁中可直接re-use這些定義好的svg, 避免重複產生 */

let svgIconContainer = document.querySelector("#svgIconContainer");
if (!svgIconContainer) {
    svgIconContainer = document.createElement("div");
    svgIconContainer.id = 'svgIconContainer';
    svgIconContainer.style = 'display:none';
    svgIconContainer.innerHTML = svgIconCollection.join('');
    document.body.prepend(svgIconContainer);
}



