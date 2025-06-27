import { Injectable } from '@angular/core';
import axios from 'axios';

@Injectable({
  providedIn: 'root'
})
export class UserAuthService {

  constructor() { }

  logingIn(username: string, password: string): Promise<any> {
    let data = {
      username: username,
      password: password
    };

    return axios.post('/login', data);
  }
}
