function showToast(type, msg) {
    if (type == "Success") {
        const toastIDSuccess = document.getElementById('successToast');
        const toastSuccess = new bootstrap.Toast(toastIDSuccess);
        $('#successToast .toast-body').html(msg);
        toastSuccess.show();
    } else if (type = "Error") {
        const toastIDError = document.getElementById('errorToast');
        const toastError = new bootstrap.Toast(toastIDError);
        $('#errorToast .toast-body').html(msg);
        toastError.show();
    }
}

function showAlert(type, msg, url) {
    if (type == "Success") {
        if (url == null || url == undefined || url == "") {
            Swal.fire({
                icon: 'success',
                title: msg,
                timer: 2000,
                showConfirmButton: false
            });
        } else {
            Swal.fire({
                icon: "success",
                iconColor: '#00A251',
                title: msg,
                html: 'You will redirect in <b></b> seconds.',
                timer: 3000,
                timerProgressBar: true,
                showCancelButton: false,
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
                didOpen: () => {
                    Swal.showLoading();
                    const timer = Swal.getPopup().querySelector("b");
                    timerInterval = setInterval(() => {
                        timer.textContent = `${Math.ceil(Swal.getTimerLeft() / 1000)}`;
                    }, 100);
                },
                willClose: () => {
                    clearInterval(timerInterval);
                    window.location.href = url;
                }
            });
        }
    } else if (type == "Error") {
        Swal.fire({
            icon: "error",
            iconColor: '#B1003C',
            title: msg,
            showCancelButton: false,
        });
    }
}

function ShowLoading(action, title, msg) {
    if (action == true) {
        Swal.fire({
            title: title,
            html: msg,
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    } else {
        Swal.close();
    }
}