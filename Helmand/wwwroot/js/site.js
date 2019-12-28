// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



const placeOrderBtn = document.querySelector('.btnPlaceOrder');


placeOrderBtn.addEventListener('click', function () {

    var name = document.getElementById('txtName').value;
    const phone = document.getElementById('txtPhone').value;
    const date = document.getElementById('datepicker').value;
    const time = document.getElementById('timepicker').value;

    if (name.toString() === '') {
        alert("Your name is required");
        return false;
    }

    else {
        if (phone.toString() === '') {
            alert("Please insert your phone number");
            return false;
        }
        else {
            if (date.toString() === '') {
                alert("Please enter a date");
                return false; 
            }
            else {
                if (time.toString() === '') {
                    alert("Please select time");
                    return false;
                }
                else {
                    return true;
                }
            }
        }

    }
});