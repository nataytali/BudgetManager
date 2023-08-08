import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, ObservableInput, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User } from '@app/_models';
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

    login(email: string, password: string) {
        return this.http.post<any>(`${environment.apiUrl}/api/auth/login`, { email, password }, {
            headers: new HttpHeaders({
              "Content-Type": "application/json"
            })
          })
            .pipe(map(user => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('currentUser', JSON.stringify(user));
                this.currentUserSubject.next(user);
                return user;
            }));
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.authToken = null;
        sessionStorage.clear();
    }

    register(firstName: string, lastname: string, email: string, password: string, confirmPassword: string)
    {
        return this.http.post<any>(`${environment.apiUrl}/api/account/register`, { firstName, lastname, email, password, confirmPassword }, {
            headers: new HttpHeaders({
              "Content-Type": "application/json"
            })
          })
        .subscribe({
          next: data => {
              //this.postId = data.id;
          },
          error: err => {
              //this.errorMessage = error.message;
              console.log('There was an error!' + err);
          }  });
    }
    private handleError(error: string) {
        if (error['status'] === 0) {
          console.error('An error occurred:', error);
        } else {
          console.error(
            `Backend returned error: ${error} `, error);
        }
        console.log(typeof error);
        return throwError(() => new Error(error));
      }
    
}