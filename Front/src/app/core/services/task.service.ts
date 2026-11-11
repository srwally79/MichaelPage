import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateTaskRequest, TaskItem, TaskStatus, UpdateTaskStatusRequest } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';

  getTasks(status?: string, userId?: string): Observable<TaskItem[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    if (userId) params = params.set('userId', userId);
    return this.http.get<TaskItem[]>(`${this.apiUrl}/tasks`, { params });
  }

  getStatuses(): Observable<TaskStatus[]> {
    return this.http.get<TaskStatus[]>(`${this.apiUrl}/statuses`);
  }

  createTask(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${this.apiUrl}/tasks`, request);
  }

  updateStatus(taskId: string, request: UpdateTaskStatusRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.apiUrl}/tasks/${taskId}/status`, request);
  }
}
