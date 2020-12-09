using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.UserProvider
{
    /// <summary>
    /// IUser interface. Contains methods related to User module.
    /// </summary>
    internal interface IUser
    {
        /// <summary>
        /// Add new User.
        /// </summary>
        /// <param name="model">Add user model.</param>
        /// <param name="password">(Optional) Password value. Default empty.</param>
        /// <returns></returns>
        Task<DataResponse<AddUserResult>> Add(AddUserModel model,string password);

        /// <summary>
        /// Update User. 
        /// </summary>
        /// <param name="userGuid">User id.</param>
        /// <param name="model">Update user model.</param>
        /// <returns></returns>
        Task<DataResponse<UpdateUserResult>> Update(string userGuid, UpdateUserModel model);

        /// <summary>
        /// Delete User.
        /// </summary>
        /// <param name="userGuid">User id.</param>
        /// <returns></returns>
        Task<DataResponse<DeleteUserResult>> Delete(string userGuid);

        /// <summary>
        /// Get User information by User Id.
        /// </summary>
        /// <param name="userGuid">User Id.</param>
        /// <param name="userInfoFlag">(Optional)User info. Default null.</param>
        /// <returns></returns>
        Task<DataResponse<ResponseGetUserDetailById>> Single(string userGuid, bool? userInfoFlag = null);

        /// <summary>
        /// Search User.
        /// </summary>
        /// <param name="model">Search filter</param>
        /// <param name="roleGuid">(Optional)User role guid. Required for search by role. Default empty.</param>
        /// <param name="entityGuid">(Optional)Entity guid. Required for search by entity. Default empty.</param>
        /// <returns></returns>
        Task<DataResponse<SearchUserResult>> Search(SearchUserModel request, string roleGuid = "", string entityGuid = "");

        /// <summary>
        /// Forgot Password.
        /// </summary>
        /// <param name="EmailId">Email Id.</param>
        /// <returns></returns>
        Task<DataResponse<ForgotPasswordResult>> ForgotPassword(string EmailId);

        /// <summary>
        /// Change Password.
        /// </summary>
        /// <param name="model">Change Password model.</param>
        /// <returns></returns>
        Task<DataResponse<ChangePasswordResult>> ChangePassword(ChangePasswordModel model);

        /// <summary>
        /// Reset Password.
        /// </summary>
        /// <param name="model">Reset Password model.</param>
        /// <returns></returns>
        Task<DataResponse<ResetPasswordResult>> ResetPassword(ResetPasswordModel model);

        /// <summary>
        /// Update User Status.
        /// </summary>
        /// <param name="userGuid">User Id.</param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<DataResponse<UpdateUserStatusResult>> UpdateUserStatus(string userGuid, bool status);

        /// <summary>
        /// Get user count.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="companyGuid">The company unique identifier.</param>
        /// <returns></returns>
        Task<DataResponse<SingleUserCountResult>> UserCount(string status, string companyGuid = "");

        Task<DataResponse<UserQuotaExhaustedNotificationResult>> GetQuotaExhaustedNotification(string token);
    }
}
