using System.ComponentModel.DataAnnotations;

namespace ZUMA.API.REST.DTOs.Authorization.Responses;

public record class AuthorizationResponseDto([MaxLength(12)] string Code);

