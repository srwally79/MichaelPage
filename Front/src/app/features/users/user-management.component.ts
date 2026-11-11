import { Component, OnInit, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { User } from '../../core/models/api.models';
import { NotificationService } from '../../core/services/notification.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './user-management.component.html'
})
export class UserManagementComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly notificationService = inject(NotificationService);

  readonly users = signal<User[]>([]);
  readonly isLoading = signal(false);
  readonly isCreating = signal(false);
  readonly createForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(255)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.userService.getUsers().subscribe({
      next: users => {
        this.users.set(users);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  createUser(): void {
    this.notificationService.clear();
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const user = this.createForm.getRawValue();
    this.isCreating.set(true);
    this.userService.createUser({
      name: user.name.trim(),
      email: user.email.trim(),
      password: user.password
    }).subscribe({
      next: createdUser => {
        this.isCreating.set(false);
        this.createForm.reset({ name: '', email: '', password: '' });
        this.notificationService.showSuccess('El usuario se creó correctamente.');
        this.users.update(users => [...users, createdUser].sort((a, b) => a.name.localeCompare(b.name)));
      },
      error: () => this.isCreating.set(false)
    });
  }
}
