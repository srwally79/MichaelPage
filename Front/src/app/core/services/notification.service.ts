import { Injectable, signal } from '@angular/core';

export interface NotificationMessage {
  kind: 'success' | 'error';
  message: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly current = signal<NotificationMessage | null>(null);

  showError(message: string): void {
    this.current.set({ kind: 'error', message });
  }

  showSuccess(message: string): void {
    this.current.set({ kind: 'success', message });
  }

  clear(): void {
    this.current.set(null);
  }
}
