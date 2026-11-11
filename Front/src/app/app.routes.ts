import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { TaskDashboardComponent } from './features/tasks/task-dashboard.component';
import { UserManagementComponent } from './features/users/user-management.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'tasks', component: TaskDashboardComponent, canActivate: [authGuard] },
  { path: 'users', component: UserManagementComponent, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: '**', redirectTo: 'tasks' }
];
