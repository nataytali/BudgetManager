import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.less']
})
export class SignupComponent implements OnInit {

  signUpForm: FormGroup;
    loading = false;
    submitted = false;
    errorMessage = '';

    constructor(
      private formBuilder: FormBuilder,
      private route: ActivatedRoute,
      private router: Router,
      private authenticationService: AuthenticationService
  ) { 
      // redirect to home if already logged in
      if (this.authenticationService.currentUserValue) { 
          this.router.navigate(['/']);
      }
  }

  ngOnInit(): void {
    this.signUpForm = this.formBuilder.group({
      firstName:['', Validators.required],
      lastName:[''],
      email: ['', [Validators.email, Validators.required]],
      password: ['', [Validators.minLength(3), Validators.required]],
      confirmPassword: ['', [Validators.required]]
  },
  {
    validator: this.ConfirmedValidator('password', 'confirmPassword'),
  });
  }
  get f() { return this.signUpForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.signUpForm.invalid) {
        return;
    }

    this.loading = true;
    this.authenticationService.register(this.f.firstName.value, this.f.lastName.value, this.f.email.value, this.f.password.value, this.f.confirmPassword.value)
            //.subscribe({
            //    next: () => {
            //         // get return url from route parameters or default to '/'
            //         const returnUrl = this.route.snapshot.queryParams['returnUrl'] || 'login';
            //         this.router.navigate([returnUrl]);
            //     },
            //     error: (err) => {
            //       this.errorMessage = err.message;
            //       console.log(err);
            //       this.loading = false;
            //     }
            // });
    
  }
  login(){
    const returnUrl = this.route.snapshot.queryParams['returnUrl'] || 'login';
    this.router.navigate([returnUrl]);
   }

   ConfirmedValidator(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];
      if (
        matchingControl.errors &&
        !matchingControl.errors.confirmedValidator
      ) {
        return;
      }
      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ confirmedValidator: true });
      } else {
        matchingControl.setErrors(null);
      }
    };
  }

}
