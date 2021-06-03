using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.DTOs;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class LikesController:BaseApiController
    {
        IUnitOfWork unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("Ypu already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to liek the user");

        }

        [HttpGet]
        public async Task<ActionResult<List<LikeDto>>> GetUserLike([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

    }
}
