function funSearchItemOrder(Controller) {
    try {
        var x = document.getElementById("Show");
        var value = x.options[x.selectedIndex].value;
        var x1 = document.getElementById("ShowSearchValue");
        var value1 = x1.options[x1.selectedIndex].value;
        var SearchString = document.getElementById("SearchString").value;
        var fromdate = document.getElementById("fromdate").value;
        var todate = document.getElementById("todate").value;
        var y = document.getElementById("id_depot");
        var id_depot = y.options[y.selectedIndex].value;
        location.replace("/" + Controller + "?Page=" + 1 + "&ID_KHO=" + id_kho + "&ShowSearchValue=" + value1 + "&SearchString=" + SearchString + "&FromDate=" + fromdate + "&ToDate=" + todate);
    }
    catch (ex) {
        alert(ex.Message);
    }
}

function OpenLoader() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "block";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function CloseLoader() {
    try {
        var modal = document.getElementById("myModal1");
        modal.style.display = "none";
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
    
}

function OnSubmit()
{
    try
    {
        OpenLoader();
        var cartList = [];
        var array = document.querySelectorAll('#tbodytmpitem input[type="checkbox"]')
        if (array != null && array.length > 0)
        {
            for (var i = 0; i < array.length; i++)
            {
                if (array[i].checked == true)
                {
                    cartList.push({
                        ID: array[i].id
                    });
                }
            }
            if (cartList != null && cartList.length > 0) {
                $.ajax({
                    url: "/Order/OnSubmitINV_DEPOSIT",
                    data: { cartOrder: JSON.stringify(cartList) },
                    dataType: "json",
                    success: OnSuccessINV_DEPOSIT,
                    error: CloseLoader
                });
            }
            else
            {
                alert("Vui lòng chọn ít nhất 1 phiếu!");
            }
        }
    }
    catch (ex)
    {
        alert(ex);
    }
}
function OnSuccessINV_DEPOSIT(data) {
    try {
        if (data.URL != null && data.URL != "")
            window.location.href = data.URL;
        else {
            if (data.Message != null && data.Message != "")
                alert(data.Message);

            location.reload();
        }
        CloseLoader();
    }
    catch (ex) {
        alert(ex.Message);
        alert(ex);
    } 
}