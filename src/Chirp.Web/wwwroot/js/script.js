/*
//the url we're on
var pageName = window.location.pathname;

document.addEventListener('DOMContentLoaded', function() {
    var followForm = document.getElementById('followForm');
    var unfollowForm = document.getElementById('unfollowForm');

    if (followForm) {
        followForm.addEventListener('submit', function(e) {
            e.preventDefault();
            console.log('Follow form submitted!');
            followOrUnfollow('Follow', followForm);
        });
    }
    
    if (unfollowForm) {
        unfollowForm.addEventListener('submit', function(e) {
            e.preventDefault();
            console.log('Unfollow form submitted!');
            followOrUnfollow('Unfollow', unfollowForm);
        });
    }
});

function followOrUnfollow(action, form) {
    fetch(`${pageName}?handler=${action}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify({ author: form.querySelector("input[name='author']").value })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        } else {
            return response.json();
        }
    })
    .then(data => {
        if (data.status === 'success') {
            const newAction = action === 'Follow' ? 'Unfollow' : 'Follow';
            form.querySelector('button').textContent = newAction;
        } else {
            console.error('Error:', data.message);
        }
    })
    .catch(e => {
        console.log('There was a problem with the fetch operation: ' + e.message);
    });
}

//error checking
console.log(JSON.stringify({ author: 'Author Name' }));
console.log(pageName);

fetch(`url`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest'
    },
    body: JSON.stringify({ author: form.querySelector("input[name='author']").value })
})*/

/*
//fetch stuff
fetch(`${pageName}?handler=Follow`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest' // This header tells the server that this is an AJAX request.
    },
    body: JSON.stringify({ author: 'Author Name' })
}).then(response => response.json()).then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    } else {
        return response.json();
    }
}).then(data => {
    if (data.status === 'success') {
        form.querySelector('button').textContent = 'Follow';
    } else {
        console.error('Error:', data.message);
    }
}).catch(e => {
    console.log('There was a problem with the fetch operation: ' + e.message);
});

fetch(`${pageName}?handler=Unfollow`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest' // This header tells the server that this is an AJAX request.
    },
    body: JSON.stringify({ author: 'Author Name' })
}).then(response => response.json()).then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    } else if (response.status === 204 || response.status === 205) {
        // 204 No Content & 205 Reset Content should not be attempted to parse
        return;
    } else {
        return response.json();
    }
}).then(data => {
    if (data.status === 'success') {
        form.querySelector('button').textContent = 'Follow';
    } else {
        console.error('Error:', data.message);
    }
}).catch(e => {
    console.log('There was a problem with the fetch operation: ' + e.message);
});*/

var pageName = window.location.pathname;

$('#followForm').submit(function(e) {
    e.preventDefault();
    $.ajax({
        url: `${pageName}?handler=Follow`,
        type: 'POST',
        data: { author: $('input[name="author"]').val() }, // send form data
        success: function(data) {
            if (data.status === 'success') {
                $('button').text('Unfollow');
            } else {
                console.error('Error:', data.message);
            }
        },
        error: function(e) {
            console.log('There was a problem with the request: ' + e.message);
        }
    });
});

// For the unfollow form

$('#unfollowForm').submit(function(e) {
    e.preventDefault();
    $.ajax({
        url: `${pageName}?handler=Unfollow`,
        type: 'POST',
        data: { author: $('input[name="author"]').val() }, // send form data
        success: function(data) {
            if (data.status === 'success') {
                $('button').text('Follow');
            } else {
                console.error('Error:', data.message);
            }
        },
        error: function(e) {
            console.log('There was a problem with the request: ' + e.message);
        }
    });
});

//errors
console.log("author " + $('input[name="author"]').val())
console.log(pageName)