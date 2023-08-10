import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, ObservableInput, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User } from '@app/_models';
import { Router, ActivatedRoute } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private currentUserSubject: BehaviorSubject<User>;
    public currentUser: Observable<User>;
    authToken: any;

    constructor(private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): User {
        return this.currentUserSubject.value;
    }

    async login(email: string, password: string) {
        try
        {
          return await fetch(`${environment.apiUrl}/api/auth/login`, {
            method: "POST",
            headers: {
              Accept: "application/json, text/plain, */*",
              "Content-Type": "application/json",
            },
           body: JSON.stringify({
            "email": email, 
            "password": password
          }), 
          })
          .then((response) => response.json())
          .then((user) => {
            console.log(user);
            localStorage.setItem('currentUser', JSON.stringify(user));
                 this.currentUserSubject.next(user);
                 return user;
            if (user.error) {
              alert("Error Password or Username"); 
            } else {
              console.log("Success")
            }
          })
          .catch((err) => {
            console.log(`Error: ${err}`);
            throw new Error(err);
          });

        }
        catch(error)
        {
          throw new Error(error)
        }
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.authToken = null;
        sessionStorage.clear();
    }

    async register(firstName: string, lastname: string, email: string, password: string, confirmPassword: string)
    {
        try
        {
          const response = await fetch(`${environment.apiUrl}/api/account/register`, {
            method: "POST", 
            mode: "cors", 
            cache: "no-cache", 
            credentials: "same-origin", 
            headers: {
              "Content-Type": "application/json",
            },
           body: JSON.stringify({ "firstName":firstName, "lastName": lastname,  "email": email, "password": password, "confirmPassword": confirmPassword }), // body data type must match "Content-Type" header
          });
          console.log("response body: " + await response.status);
          return await response; 
        }
        catch(error)
        {
          throw new Error(error)
        }
        
    }

}