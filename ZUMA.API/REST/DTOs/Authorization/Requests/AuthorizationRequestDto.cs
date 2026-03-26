using System.ComponentModel.DataAnnotations;

namespace ZUMA.API.REST.DTOs.Authorization.Requests;

public record class AuthorizationRequestDto([EmailAddress] string Email);

