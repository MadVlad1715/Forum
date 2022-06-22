﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DataTransferObjects;
using Service.Interfaces;

namespace WebApi.Controllers
{
    /// <summary>
    /// Users controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor for initializing a <see cref="UsersController"/> class instance
        /// </summary>
        /// <param name="userService">User service</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>Array of users</returns>
        /// <response code="200">Returns the array of users</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserWithDetailsDto>>> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        /// <summary>
        /// Updates user role
        /// </summary>
        /// <param name="id">Guid of the user whose role is to be updated</param>
        /// <param name="roleDto">Role update data</param>
        /// <remarks>Unable to update role of administrators</remarks>
        /// <response code="204">User role has been updated</response>
        /// <response code="400">Failed to update user role, error returned</response>
        /// <response code="404">User specified by <paramref name="id"/> not found</response>
        [HttpPut("{id:guid}/role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateRole(Guid id, [FromBody] UserRoleUpdateDto roleDto)
        {
            await _userService.UpdateRoleAsync(id, roleDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes the user
        /// </summary>
        /// <param name="id">Guid of the user to be deleted</param>
        /// <remarks>Unable to delete administrators</remarks>
        /// <response code="204">User has been deleted</response>
        /// <response code="400">Unable to delete administrator</response>
        /// <response code="404">User specified by <paramref name="id"/> not found</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}