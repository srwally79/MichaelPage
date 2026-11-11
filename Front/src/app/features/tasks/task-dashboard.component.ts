import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, JsonPipe, NgClass } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, ValidatorFn } from '@angular/forms';
import { TaskItem, TaskStatus, User } from '../../core/models/api.models';
import { NotificationService } from '../../core/services/notification.service';
import { TaskService } from '../../core/services/task.service';

const jsonObjectValidator: ValidatorFn = control => {
  if (!control.value) return null;
  try {
    const value: unknown = JSON.parse(control.value as string);
    return value !== null && typeof value === 'object' && !Array.isArray(value)
      ? null
      : { jsonObject: true };
  } catch {
    return { jsonInvalid: true };
  }
};

@Component({
  selector: 'app-task-dashboard',
  standalone: true,
  imports: [DatePipe, JsonPipe, NgClass, ReactiveFormsModule],
  templateUrl: './task-dashboard.component.html'
})
export class TaskDashboardComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly notificationService = inject(NotificationService);

  readonly tasks = signal<TaskItem[]>([]);
  readonly users = signal<User[]>([]);
  readonly statuses = signal<TaskStatus[]>([]);
  readonly isLoading = signal(false);
  readonly isCreating = signal(false);
  readonly updatingTaskId = signal<string | null>(null);

  readonly filterForm = this.formBuilder.nonNullable.group({
    status: [''],
    userId: ['']
  });

  readonly createForm = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(255)]],
    userId: ['', Validators.required],
    additionalInfo: ['{}', [Validators.required, jsonObjectValidator]]
  });

  ngOnInit(): void {
    this.loadUsers();
    this.loadStatuses();
    this.loadTasks();
  }

  loadTasks(): void {
    const { status, userId } = this.filterForm.getRawValue();
    this.isLoading.set(true);
    this.taskService.getTasks(status || undefined, userId || undefined).subscribe({
      next: tasks => {
        this.tasks.set(tasks);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  createTask(): void {
    this.notificationService.clear();
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const value = this.createForm.getRawValue();
    this.isCreating.set(true);
    this.taskService.createTask({
      title: value.title.trim(),
      userId: value.userId,
      additionalInfo: JSON.parse(value.additionalInfo) as Record<string, unknown>
    }).subscribe({
      next: () => {
        this.isCreating.set(false);
        this.createForm.reset({ title: '', userId: '', additionalInfo: '{}' });
        this.notificationService.showSuccess('La tarea se creó correctamente.');
        this.loadTasks();
      },
      error: () => this.isCreating.set(false)
    });
  }

  updateStatus(task: TaskItem, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;
    if (!status || status === task.status) return;

    this.updatingTaskId.set(task.id);
    this.taskService.updateStatus(task.id, { status }).subscribe({
      next: () => {
        this.updatingTaskId.set(null);
        this.notificationService.showSuccess('El estado de la tarea se actualizó.');
        this.loadTasks();
      },
      error: () => {
        this.updatingTaskId.set(null);
        this.loadTasks();
      }
    });
  }

  statusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  private loadUsers(): void {
    this.taskService.getUsers().subscribe({
      next: users => this.users.set(users),
      error: () => this.users.set([])
    });
  }

  private loadStatuses(): void {
    this.taskService.getStatuses().subscribe({
      next: statuses => this.statuses.set(statuses),
      error: () => this.statuses.set([])
    });
  }
}
