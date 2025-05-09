function funSearchInputOutput(Controller) {
    try {
        var x = document.getElementById("Show");
        var value = x.options[x.selectedIndex].value;
        var SearchString = document.getElementById("SearchString").value;
        var fromdate = document.getElementById("fromdate").value;
        var todate = document.getElementById("todate").value;
        location.replace("/" + Controller + "?Page=" + 1 + "&SearchString=" + SearchString + "&FromDate=" + fromdate + "&ToDate=" + todate);
    }
    catch (ex) {
        alert(ex.Message);
    }
}
