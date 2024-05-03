export default interface IGetUserDto{
    applicationUserId: string;
    login: string;
    roles: Array<number>;
}