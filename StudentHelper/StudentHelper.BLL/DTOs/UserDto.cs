// StudentHelper.BLL/DTOs/UserDto.cs
namespace StudentHelper.BLL.DTOs
{
    public record UserDto(
        int Id,
        string FirstName,
        string LastName,
        string Email
    );
}