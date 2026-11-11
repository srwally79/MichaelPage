import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgClass, RouterOutlet],
  templateUrl: './app.component.html'
})
export class AppComponent {
  readonly authService = inject(AuthService);
  readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  dismissNotification(): void {
    this.notificationService.clear();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => void this.router.navigateByUrl('/login'),
      error: () => void this.router.navigateByUrl('/login')
    });
  }
}
