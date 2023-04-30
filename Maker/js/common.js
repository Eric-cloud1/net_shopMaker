// JScript File
function initAjaxProgress()
{
    var pageHeight = (document.documentElement && document.documentElement.scrollHeight) ? document.documentElement.scrollHeight : (document.body.scrollHeight > document.body.offsetHeight) ? document.body.scrollHeight : document.body.offsetHeight;
    //SET HEIGHT OF BACKGROUND
    var bg = document.getElementById('ajaxProgressBg');
    bg.style.height = (pageHeight + 1000) + 'px';
    //POSITION THE PROGRESS INDICATOR ON INITIAL LOAD
    reposAjaxProgress();
    //REPOSITION THE PROGRESS INDICATOR ON SCROLL
    window.onscroll = reposAjaxProgress;
}

function reposAjaxProgress()
{
    var div = document.getElementById('ajaxProgress');
    var st = document.body.scrollTop;
    if (st == 0) {
        if (window.pageYOffset) st = window.pageYOffset;
        else st = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
    }
    div.style.top = 150 + st + "px";
}