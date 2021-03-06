import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor, HttpStatusCode
} from '@angular/common/http';
import {catchError, EMPTY, Observable, throwError} from 'rxjs';
import {AuthService} from "../services/auth.service";
import {environment as env} from "../../environments/environment";
import {Router} from "@angular/router";
import {NotificationService} from "../services/notification.service";

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService,
              private ns: NotificationService,
              private router: Router) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    request = request.clone({url: `${env.apiUrlPrefix}${request.url}`});

    if (this.auth.isLoggedIn) {
      const token = this.auth.session?.accessToken!;

      request = request.clone({
        headers: request.headers.set('Authorization', `Bearer ${token}`)
      });
    }

    return next.handle(request).pipe(
      catchError(err => {
        if (err.status === HttpStatusCode.Unauthorized && this.auth.isLoggedIn) {
          this.auth.signOut();
          this.router.navigate(['/sign-in']);
          this.ns.notifyError("Session expired, please sign in again");
          return EMPTY;
        }

        return throwError(err);
      })
    );
  }
}
