var toolTipTimer = null;

function ShowToolTip(obj, delay, hoverClass)
{
    toolTipTimer = setTimeout("ShowToolTipTimer('" + obj.id + "','" + hoverClass + "')", delay);
}
            
function HideToolTip(obj, normalClass)
{
    clearTimeout(toolTipTimer);
    obj.className = normalClass;
    obj.getElementsByTagName('iframe')[0].style.display='none';
}

function ShowToolTipTimer(objid, hoverClass)
{
	obj = document.getElementById(objid);
	if (obj != null) obj.className = hoverClass;
    var frm = obj.getElementsByTagName('iframe')[0];
    var divtg = obj.getElementsByTagName('div')[0];
    frm.style.display='block';
    frm.style.width = divtg.offsetWidth;
    frm.style.height = divtg.offsetHeight;
    frm.style.left = '0px';
    frm.style.top = divtg.offsetTop;
}