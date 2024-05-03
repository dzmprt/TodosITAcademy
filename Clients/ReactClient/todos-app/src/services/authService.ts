import axios, { AxiosResponse } from "axios";
import IJwtTokenDto from "../models/apiDto/jwtTokenDto";
import IGetUserDto from "../models/apiDto/getUserDto";

const loginJwtUrl: string = 'http://localhost/auth/api/v1/LoginJwt';

const refreshJwtUrl: string = 'http://localhost/auth/api/v1/RefreshJwt';

const currentUserUrl: string = 'http://localhost/auth/api/v1/Users/Current';

const controller = new AbortController();

class AuthService {
    public async singIn(login: string, password: string): Promise<boolean> {


        try {
            const result = await axios.post<IJwtTokenDto>(
                loginJwtUrl,
                {
                    login,
                    password
                },
                {
                    signal: controller.signal,
                    headers: {
                        'Access-Control-Allow-Origin' : '*'
                    },
                }
            );
            localStorage.setItem('jwt', JSON.stringify(result.data));
            return true;
        }
        catch {
            return false;
        }
    }

    public async refreshJwt(): Promise<boolean> {

        const jwtString = localStorage.getItem('jwt');
        if (jwtString == null) {
            return false;
        }

        const jwt: IJwtTokenDto = JSON.parse(jwtString);

        try {
            const result = await axios.post<IJwtTokenDto>(
                refreshJwtUrl,
                {
                    refreshToken: jwt.refreshToken
                },
                {
                    signal: controller.signal
                }
            );

            localStorage.setItem('jwt', JSON.stringify(result.data));
            return true;
        }
        catch {
            return false;
        }
    }

    public async getCurrentUser() : Promise<IGetUserDto | null>{

        const jwtString = localStorage.getItem('jwt');
        if (jwtString == null) {
            return null;
        }
        const jwt: IJwtTokenDto = JSON.parse(jwtString);

        try {
            const result = await axios.get<IGetUserDto>(
                currentUserUrl,
                {
                    headers: {
                        'Authorization' : `Bearer ${jwt.jwtToken}`,
                        'Access-Control-Allow-Origin' : '*'
                    },
                    signal: controller.signal
                }
            );

            return result.data;
        }
        catch {
            return null;
        }
    } 

}

const authService: AuthService = new AuthService();
export default authService;