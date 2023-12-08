//follow and unfollow
$(document).ready(function() {
    $(".followForm, .unfollowForm").on('submit', function(e) {
        e.preventDefault();
        let form = $(this);
        let url = form.attr('action');
        let button = form.find('button');
        let author = form.data('author'); // Get the author related to this form

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            success: function(response)
            {
                if(response.success) {
                    let newAction = button.hasClass('follow') ? 'Unfollow' : 'Follow';
                    let newClass = button.hasClass('follow') ? 'unfollow' : 'follow';
                    let newHandler = button.hasClass('follow') ? 'Unfollow' : 'Follow';

                    // Target all forms and buttons related to the author
                    $('.followForm[data-author="' + author + '"], .unfollowForm[data-author="' + author + '"]').each(function() {
                        let thisForm = $(this);
                        let thisButton = thisForm.find('button');

                        thisButton.text(newAction);
                        thisButton.removeClass('follow unfollow').addClass(newClass);
                        thisForm.attr('asp-page-handler', newHandler);
                        thisForm.removeClass('followForm unfollowForm').addClass(newClass + 'Form');
                        thisForm.attr('action', '/?handler=' + newHandler);
                    });
                }
            },
            error: function(error) {
                console.log('Error:', error);
            }
        });
        return false;  // prevent form from submitting normally
    });
});
//upvote and downvote
$(document).ready(function() {
    $(".vote-btn").on('submit', function(e) {
        e.preventDefault();
        let form = $(this);
        let url = form.attr('action');
        let button = form.find('button');
        let voteType = form.find('input[name="NewReaction.Reaction"]').val();
        let id = form.data('id'); // Get the id related to this form

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            success: function(response)
            {
                if(response.success) {
                    // get the span within the button and update
                    let voteCountSpan = button.find('span');
                    voteCountSpan.text(voteType === 'Upvote' ? response.upVoteCount : response.downVoteCount);

                    // get the span within the opposite button and update
                    let oppositeVoteType = voteType === 'Upvote' ? 'Downvote' : 'Upvote';
                    let oppositeButton = $("button:contains('" + oppositeVoteType + "')[data-id='" + id + "']");
                    let oppositeVoteCountSpan = oppositeButton.find('span');
                    oppositeVoteCountSpan.text(oppositeVoteType === 'Upvote' ? response.upVoteCount : response.downVoteCount);
                }
            },
            error: function(error) {
                console.log('Error:', error);
            }
        });
        return false;  // prevent form from submitting normally
    });
});