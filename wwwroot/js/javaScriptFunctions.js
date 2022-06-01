
  function printImg() {
        pwin = window.open();
      pwin.document.write( '<html<body><img src="images/barcodepallete.png"></body></html>' );
      setTimeout( function ()
      {
          pwin.window.print();
          pwin.document.close();
      }, 1000 );
     


        //popup = window.open();
        //popup.document.write( "imagehtml" );
        //popup.focus(); //required for IE
        //popup.print();
    }

//function printImg ()
//{
  
//    var w = window.open();
//    //var printOne = document.getElementById( "barcodeImg" ).innerHTML();
//       var printTwo = $( '#barcodeImg' ).html();
//        w.document.write( '<html><head><title>Copy Printed</title></head><body><h1>Copy Printed</h1><hr />' + printTwo) + '</body></html>';
//        w.window.print();
//        w.document.close();
//        return false;
    
//}

function focusEditor(className) {
    document.getElementsByClassName(className)[0].querySelector("input").focus();
}

function focusEditorByID ( id )
{
    document.getElementById( id ).querySelector( 'input' ).focus();
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

function printSVG ( className, svg )
{
    w = window.open();
    w.document.open();
    w.document.write( "<div class='viewport'>" );
    w.document.write( svg );
    w.document.write( "</div>" );
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
function GetValueTextBox ( id, text )
{
    return document.getElementById( id ).querySelector( "input" ).value;
}
function SetValueTextBox(className, text) {
    document.getElementsByClassName(className)[0].querySelector("input").value = text;
}


function CheckEditorDisable ( className )
{
    return document.getElementsByClassName( className )[ 0 ].querySelector( "input" ).disable;
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