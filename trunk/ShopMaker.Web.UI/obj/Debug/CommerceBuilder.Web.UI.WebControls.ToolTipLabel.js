var toolTipTimer = null;

function ShowToolTip(obj, delay, hoverClass)
{
    toolTipTimer = setTimeout("ShowToolTipTimer('" + obj.id + "','" + hoverClass + "')", delay);
}
            
function HideToolTip(obj, normalClass)
{
    clearTimeout(toolTipTimer);
    obj.className = normalClass;
}

function ShowToolTipTimer(objid, hoverClass)
{
	obj = document.getElementById(objid);
	if (obj != null) obj.className = hoverClass;
}