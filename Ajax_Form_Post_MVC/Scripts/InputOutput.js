function funSearchInputOutput(Controller) {
    try {
        var x = document.getElementById("Show");
        var value = x.options[x.selectedIndex].value;
        var SearchString = document.getElementById("SearchString").value;
        var fromdate = document.getElementById("fromdate").value;
        var todate = document.getElementById("todate").value;
        var s = document.getElementById("id_depot");
        var id_depot = s ? s.options[s.selectedIndex].value : "";
        s = document.getElementById("lstID_KHUVUCSEARCH");
        var ID_KHUVUC = s ? s.options[s.selectedIndex].value : "";

        var srch_ID_KHUVUC =  "&SearchString=" + SearchString + "&FromDate=" + fromdate + "&ToDate=" + todate;
        if (id_depot != null && id_depot != "") {
            srch_ID_KHUVUC += "&ID_DEPOT=" + id_depot;
        }
        if (ID_KHUVUC != null && ID_KHUVUC != "") {
            srch_ID_KHUVUC += "&ID_KHUVUC=" + ID_KHUVUC;
        }
        location.replace("/" + Controller + "/Index?Page=" + 1 + srch_ID_KHUVUC);
    }
    catch (ex) {
        alert(ex.Message);
    }
}
function OnSubmitSales(ControllerName) 
{
    try {
        var cartList = [];
        var array = document.querySelectorAll('#tbodytmpitem input[name="item.CHON"]');

        for (var i = 0; i < array.length; i++) {
            if (array[i].checked) {
                cartList.push({ ID: array[i].id });
            }
        }

        if (cartList.length === 0) {
            alert("Vui lòng chọn ít nhất một đơn hàng.");
            return;
        }

        // Tạo form ẩn để submit
        var form = document.createElement("form");
        form.method = "POST";
        form.action = "/" + ControllerName + "/LoadData"; // Thay ControllerName

        var input = document.createElement("input");
        input.type = "hidden";
        input.name = "cartOrder";
        input.value = JSON.stringify(cartList);

        form.appendChild(input);
        document.body.appendChild(form);
            form.submit();
        }
    catch (ex) {
        alert(ex.Message);
    }
}