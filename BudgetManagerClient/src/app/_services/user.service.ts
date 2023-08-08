import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { User } from '@app/_models';
import {AuthenticationService} from '@app/_services'

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    async getAll() {

        const authService = new AuthenticationService(this.http);

        const token = authService.currentUserValue['accessToken'];

        const options: RequestInit = {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
              },
            mode: "cors",
            cache: "default",
        };

        const response = await fetch(`${environment.apiUrl}/api/account`, options);
        var users = await response.json();
        return users;
    }
}