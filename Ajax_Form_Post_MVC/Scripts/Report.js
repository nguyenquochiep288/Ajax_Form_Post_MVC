function myFunctionReport()
{
    try {
        var input, filter, ul, li, a, i, txtValue;
        input = document.getElementsByClassName("sui-treeview-list");
        filter = input.value.toUpperCase();
        ul = document.getElementById("treeview");
        li = ul.querySelectorAll('li');
        for (i = 0; i < li.length; i++) {
            a = li[i].getElementsByTagName("a")[0];
            txtValue = li[i].textContent || li[i].innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                li[i].style.visibility = "visible";
            } else {
                li[i].style.visibility = "hidden";
            }
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function OnSuccessReport(ID) {
    try {
        CloseLoaderReport();
        //$("#reportView" + ID).attr("src", "VerReporte").load();
        // Kiểm tra thiết bị di động
        var isMobile = /Android|iPhone|iPad|iPod|Opera Mini|IEMobile|WPDesktop/i.test(navigator.userAgent);
        var ua = navigator.userAgent;

        // Kiểm tra Safari (nhưng không phải Chrome)
        var isSafari = /^((?!chrome|android).)*safari/i.test(ua);
        if (isMobile && !isSafari) {
            const myWindow = window.open('/ViewReport/GetReport', 'MyWindow',
                'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,width=900,height=600');
        } else {
            // Thiết bị không phải di động
            $("#reportView" + ID).attr("src", "VerReporte").load();
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function OnSuccessReportMobile() {
    try {
        // Mở cửa sổ pop-up cho thiết bị di động
        window.open('/ViewReport/GetReport', 'MyWindow',
            'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,width=900,height=600');
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    }

}

function DeleteTab(idTab)
{
    try {
        var divElem = document.getElementById("content" + idTab);
        divElem.remove();

        var divElem = document.getElementById("tab" + idTab);
        divElem.remove();

        divElem = document.getElementById("tabhome");
        divElem.className = "active";

        divElem = document.getElementById("contenthome");
        divElem.className = "tab-pane active";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function AddTab(idTab)
{
    try {
        var divElem = document.getElementById("tabhome");
        divElem.className = "";
        divElem = document.getElementById("contenthome");
        divElem.className = "tab-pane";

        divElem = document.getElementById("tab" + idTab);
        if (divElem != null) {
            divElem.className = "active";
            divElem = document.getElementById("content" + idTab);
            if (divElem != null)
                divElem.className = "tab-pane active";
            return;
        }


        OpenLoaderReport();
        $.ajax({
            type: "GET",
            url: "/ViewReport/AddTab?ID=" + idTab,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnSuccessAddTab,
            error: OnErrorLoadAddTab
        });
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}
function OnSuccessAddTab(apiResponse) {
    try
    {
        var i, j, m, n = 0;
        CloseLoaderReport();
        if (apiResponse.Success)
        {
            $('#tabhome').after(apiResponse.TAB);
            var divElem = document.getElementById("tabhome");
            divElem.className = "";
            divElem = document.getElementById("contenthome");
            divElem.className = "tab-pane";

            divElem = document.getElementById("content" + apiResponse.ID);
            if (divElem == null || divElem.length == 0)
            { 
                divElem = document.getElementById("contenthome1");
                var newcloneNode = divElem.cloneNode(true);
                newcloneNode.id = "content" + apiResponse.ID;
                newcloneNode.name = apiResponse.ID;
                var divparameter = newcloneNode.querySelectorAll('#parameter_');
                if (divparameter.length > 0)
                {
                    for (i = 0; i < divparameter.length; i++)
                    {
                        divparameter[i].id = "parameter" + apiResponse.ID;
                        divparameter[i].innerHTML = apiResponse.CONTENT;
                    }
                }
                var divReport = newcloneNode.querySelectorAll('#reportView1');
                if (divReport.length > 0) {
                    for (i = 0; i < divReport.length; i++) {
                        divReport[i].id = "reportView" + apiResponse.ID;
                    }
                }
                newcloneNode.style.visibility = '';
                newcloneNode.style.display = '';
                newcloneNode.className = "tab-pane active";
                $('#contenthome').after(newcloneNode);
            }

            //$('.chosen-select').attr('data-live-search', 'true');
            //$('.chosen-select').attr('data-style', 'form-control');
            $('.chosen-select').chosen({
                allow_single_deselect: true
            });
           
        }
        else
        {
            if (apiResponse.URL != null && apiResponse.URL != "")
                window.location.href = apiResponse.URL;
            else
                alert(apiResponse.Message);

            var divElem = document.getElementById("tabhome");
            divElem.className = "active";
            divElem = document.getElementById("contenthome");
            divElem.className = "tab-pane active";
        }
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
        CloseLoaderReport();
    }
}

function OnErrorLoadAddTab() {
    alert("Lỗi: vui lòng liên hệ nhà cung cấp! Xin cảm ơn.");
    CloseLoaderReport();
}

function OpenLoaderReport() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "block";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
   
}

function CloseLoaderReport() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "none";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}