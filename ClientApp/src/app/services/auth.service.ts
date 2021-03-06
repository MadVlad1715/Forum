import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {tap} from "rxjs";
import {Session} from "../interfaces/session";
import {JwtHelperService} from "@auth0/angular-jwt";
import {NotificationService} from "./notification.service";
import {SignInData} from "../interfaces/sing-in-data";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();

  constructor(private api: HttpClient,
              private ns: NotificationService,
              private roter: Router) {
    if (this.isLoggedIn && this.jwtHelper.isTokenExpired(this.session?.accessToken)) {
      this.signOut();
    }
  }

  public signIn(login: string, password: string) {
    return this.api.post<Session>('auth/sign-in', <SignInData>{login, password})
      .pipe(
        tap(this.setSession)
      );
  }

  public signOut() {
    localStorage.removeItem('session');

    switch (this.roter.url) {
      case '/users':
      case '/profile':
        this.roter.navigate(['/']);
    }
  }

  private setSession(session: Session) {
    localStorage.setItem('session', JSON.stringify(session));
  }

  get isLoggedIn(): boolean {
    return this.session !== undefined;
  }

  get session(): Session | undefined {
    const session = localStorage.getItem('session');

    if (session == null) {
      return;
    }

    return JSON.parse(session);
  }
}
