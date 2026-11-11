import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of, tap, throwError } from 'rxjs';
import { AuthSession, LoginRequest, LoginResponse } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';
  private readonly authenticated = signal(false);

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, request).pipe(
      tap(() => this.authenticated.set(true))
    );
  }

  checkSession(): Observable<boolean> {
    if (this.authenticated()) return of(true);
    return this.http.get<AuthSession>(`${this.apiUrl}/auth/session`).pipe(
      map(() => {
        this.authenticated.set(true);
        return true;
      }),
      catchError(() => {
        this.authenticated.set(false);
        return of(false);
      })
    );
  }

  isAuthenticated(): boolean {
    return this.authenticated();
  }

  markUnauthenticated(): void {
    this.authenticated.set(false);
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {}).pipe(
      tap(() => this.authenticated.set(false)),
      catchError(error => {
        this.authenticated.set(false);
        return throwError(() => error);
      })
    );
  }
}
