using ERMS.DTOs.Profile;

namespace ERMS.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto> GetProfileAsync(int employeeId);
        Task<UpdateResultDto> UpdatePersonalInfoAsync(UpdatePersonalInfoDto dto);
        Task<UpdateResultDto> UpdatePasswordAsync(UpdatePasswordDto dto);
        Task<UpdateResultDto> UpdateProfilePictureAsync(UpdateProfilePictureDto dto);
    }
}
