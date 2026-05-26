$('#btnAddToCart').on('click', function () {
    var iconSize = parseInt($('#txtCartCount').attr('size'));
    var customerID = localStorage.getItem('customerID');
    if (customerID == null) {
        customerID = 0;
    }

    $.ajax({
        type: "post",
        url: urlAddToCart,
        data: {
            customerID: customerID,
            productID: productID,
            colorway: colorway
        },
        datatype: "json",
        traditional: true,
        success: function (data) {
            var count = Number(data);
            if (count >= 10 && count < 100) {
                iconSize += 2;
                $('#txtCartCount').css({
                    'height': iconSize,
                    'width': iconSize
                });

                $('#txtCartCount > text').html(count);
            }
            else if (count >= 100) {
                iconSize += 4;
                $('#txtCartCount').css({
                    'height': iconSize,
                    'width': iconSize
                });

                $('#txtCartCount > text').html("99+");
            }
            else {
                $('#txtCartCount > text').html(count);
            }
            $('#txtCartCount > text').attr('count', count);

            const toastLiveExample = document.getElementById('successToast');
            const toast = new bootstrap.Toast(toastLiveExample);
            toast.show();
        },
    });
});