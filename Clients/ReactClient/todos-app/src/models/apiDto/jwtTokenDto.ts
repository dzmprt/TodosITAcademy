export default interface IJwtTokenDto{
    jwtToken: string;
    refreshToken: string;
    expires: Date;
}