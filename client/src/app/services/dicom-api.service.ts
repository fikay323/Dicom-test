import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class DicomApiService {
  private readonly BASE_URL = 'http://localhost:5245/api/dicom';

  constructor(private readonly http: HttpClient) {}

  uploadDicomFile(file: File): Observable<{ filename: string }> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<{ filename: string }>(`${this.BASE_URL}/upload`, formData)
      .pipe(catchError(this.handleError));
  }

  renderDicomFile(filename: string): Observable<Blob> {
    const url = `${this.BASE_URL}/render?filename=${encodeURIComponent(filename)}`;
    return this.http.get(url, { responseType: 'blob' })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    const message = error.error?.message || error.message || 'Unknown error occurred';
    return throwError(() => new Error(message));
  }
}
