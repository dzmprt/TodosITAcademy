import { useState } from 'react';
import authService from '../../services/authService';
import './index.css'
import IGetUserDto from '../../models/apiDto/getUserDto';

export default function AuthForm() {

    const [currentUser, setCurrentUser] = useState<IGetUserDto | null>(null);

    const [error, setError] = useState<string>('');

    const handlers = {
        onFormSend: (event: React.SyntheticEvent<HTMLFormElement>) => {
            event.preventDefault();
            const target = event.target as typeof event.target & {
                login: { value: string };
                password: { value: string };
            };
            authService.singIn(target.login.value, target.password.value)
                .then((resp) => {
                    setError('');
                    authService.getCurrentUser()
                        .then((user) => setCurrentUser(user));
                })
                .catch((error) => {
                    setError('Ошибка авторизации')
                });

        }
    }

    return (
        <>
            {
                currentUser != null && <h1>{currentUser?.login}</h1>
            }
            <form onSubmit={handlers.onFormSend} className="sing-in-form" >
                <label>Login</label>
                <input name="login" type="text" />
                <label>Password</label>
                <input name="password" type="password" />
                <button type="submit">Sing-in</button>
                {error.length > 0 &&
                    <label className='errorLabel'>{error}</label>
                }

            </form>
        </>

    );
}