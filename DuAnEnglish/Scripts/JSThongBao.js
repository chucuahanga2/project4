// Hàm hiển thị thông báo SweetAlert2
function showAlert(message) {
    Swal.fire({
        icon: 'info',
        title: 'Thông báo',
        text: message,
        background: '#ecf0f1',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'OK',
        showCancelButton: false,
        timer: 5000,
        customClass: {
            popup: 'swal2-popup',
            title: 'swal2-title',
            content: 'swal2-content',
            confirmButton: 'swal2-confirm',
            cancelButton: 'swal2-cancel'
        },
        didOpen: () => {
            document.body.style.overflow = 'hidden'; // Ngăn cuộn khi popup mở
        },
        willClose: () => {
            document.body.style.overflow = ''; // Khôi phục cuộn khi popup đóng
        }
    });
}
