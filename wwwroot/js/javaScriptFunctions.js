
function onCustomizeExportOptions(s, e) {
    e.HideProperties(DevExpress.Reporting.Viewer.ExportFormatID.XLS, "ExportMode", "PageRange");
    e.HideProperties(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
}

function printImg() {
    pwin = window.open();
    pwin.document.write('<html<body><img src="images/barcodepallete.png"></body></html>');
    setTimeout(function () {
        pwin.window.print();
        pwin.document.close();
    }, 1000);
}


function playSound(src) {
    var audio = document.getElementById('player');
    if (audio != null) {
        var audioSource = document.getElementById('playerSource');
        if (audioSource != null) {
            audioSource.src = src;
            audio.load();
            audio.play();
        }
    }
}

function playSounds(src) {
    var audio = document.createElement('audio');
    if (audio != null) {
        var audioSource = document.createElement('audioSource');
        if (audioSource != null) {
            audio.type = 'audio/mpeg';
            audioSource.src = src;
            audio.load();
            audio.play();
        }
    }
}

function focusEditor(className) {
    document.getElementsByClassName(className)[0].querySelectorAll('input').focus();
}

function focusEditorByID(Id) {
    setReadOnlyByID(Id, false);
    if (document.getElementById(Id) != null && document.getElementById(Id).querySelector('input') != null)
        document.getElementById(Id).querySelector('input').focus();
}



function setReadOnlyByID(Id, status) {
    if (document.getElementById(Id) != null && document.getElementById(Id).querySelector('input') != null) {
        document.getElementById(Id).querySelector('input').readOnly = status;
    }

}

function getPathFile(id) {
    return document.getElementById(id).value;
}

function saveAsFile(fileName, byteBase64) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function printHtml(id) {
    var htmlcontent = document.getElementById(id).innerHTML;
    var a = window.open('', '', '');
    a.document.write('<html>');
    a.document.write('<body><br/>');
    a.document.write(htmlcontent);
    a.document.write('<body></html>');
    a.document.close();
    a.print();
}

function printLocal(className) {
    w = window.open();
    w.document.open();
    w.document.write("<html><head></head><body>");
    w.document.write("HI");
    w.document.write("</body></html>");
    w.document.close();
    w.print();
    w.close();
}

function printSVG(className, svg) {
    w = window.open();
    w.document.open();
    w.document.write("<div class='viewport'>");
    w.document.write(svg);
    w.document.write("</div>");
    w.document.close();
    w.print();
    w.close();
}

function ConsoleLog(object) {
    console.log(object);
}

function ShowText(id, text) {
    document.getElementById(id).innerHTML = text;
}
function GetValueTextBox(id, text) {
    return document.getElementById(id).querySelector("input").value;
}
function SetValueTextBox(className, text) {
    document.getElementsByClassName(className)[0].querySelector("input").value = text;
}


function CheckEditorDisable(className) {
    return document.getElementsByClassName(className)[0].querySelector("input").disable;
}

function AddOrRemoveChecked(id, status) {
    if (status == 1) {
        // Create a href attribute:
        const att = document.createAttribute("checked");

        // Set the value of the href attribute:
        att.value = "checked";

        // Add the href attribute to an element:
        document.getElementById(id).setAttributeNode(att);
    } else {
        document.getElementById(id).removeAttribute("checked");
    }
}

window.Remove = {
    headerFromTimeline: function () {
        var wrapperElements = document.getElementsByClassName('dxbs-sc-tb-wrapper');
        var scrollElements = document.getElementsByClassName('dxbs-sc-v-scroll-spacer');

        while (wrapperElements.length > 0) {
            wrapperElements[0].parentNode.removeChild(wrapperElements[0]);
        }

        while (scrollElements.length > 0) {
            scrollElements[0].parentNode.removeChild(scrollElements[0]);
        }
    }
};