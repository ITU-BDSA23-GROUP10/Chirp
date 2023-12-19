namespace Chirp.Core;
// The FollowDTO is used to send Follow data to the frontend for displaying.
public record FollowDTO ( int followerId, int followingId );