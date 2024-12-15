$(document).ready(function () {
    GetCategoryDetails();
});

function getCookie(name) {
    const cookieValue = document.cookie
        .split('; ')
        .find(cookie => cookie.startsWith(name + '='))
        ?.split('=')[1];
    return cookieValue ? decodeURIComponent(cookieValue) : null;
}
function GetCategoryDetails() {
    // Get jwtToken cookie value
    // Retrieve 'jwtToken' cookie value
    const jwtToken = getCookie('jwtToken');
    if (!jwtToken) {
        // Redirect to login page if jwtToken cookie is not available
        window.location.href = '/auth/login';
        return; // Stop further execution
    }
    $('#loader').show();
    var currentUrl = window.location.href;
    var id = currentUrl.substring(currentUrl.lastIndexOf('/') + 1);
    $.ajax({
        url: "http://localhost:5144/api/Category/GetCategoryById/" + id,
        type: 'GET',
        dataType: 'json',
        headers: {
            'Authorization': 'Bearer ' + jwtToken
        },
        success: function (response) {
            if (response.success) {
                $("#categoryName").html(response.data.categoryName);
                $("#categoryDescription").html(response.data.categoryDescription);
            }
        },
        error: function (xhr, status, error) {
            // Check if there is a responseText available
            if (xhr.responseText) {
                try {
                    // Parse the responseText into a JavaScript object
                    var errorResponse = JSON.parse(xhr.responseText);

                    // Check the properties of the errorResponse object
                    if (errorResponse && errorResponse.message) {
                        // Display the error message to the user
                        alert('Error: ' + errorResponse.message);
                    } else {
                        // Display a generic error message
                        alert('An error occurred. Please try again.');
                    }
                } catch (parseError) {
                    console.error('Error parsing response:', parseError);
                    alert('An error occurred. Please try again.');
                }
            } else {
                // Display a generic error message if no responseText is available
                alert('An unexpected error occurred. Please try again.');
            }
        },
        complete: function () {
            $('#loader').hide();
        }
    });
}