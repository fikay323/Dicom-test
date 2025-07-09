import { Component } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { DicomApiService } from '../../services/dicom-api.service';

@Component({
  selector: 'app-viewer',
  templateUrl: './viewer.component.html',
  styleUrls: ['./viewer.component.scss']
})
export class ViewerComponent {
  selectedFile: File | null = null;
  previewUrl: SafeUrl | null = null;
  loading = false;
  error: string | null = null;

  constructor(private readonly dicomApi: DicomApiService, private readonly sanitizer: DomSanitizer) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  upload(): void {
    if (!this.selectedFile) return;

    this.loading = true;
    this.error = null;

    this.dicomApi.uploadDicomFile(this.selectedFile).subscribe({
      next: res => {
        this.dicomApi.renderDicomFile(res.filename).subscribe({
          next: blob => {
            const url = URL.createObjectURL(blob);
            this.previewUrl = this.sanitizer.bypassSecurityTrustUrl(url);
            this.loading = false;
          },
          error: err => {
            this.error = err.message;
            this.loading = false;
          }
        });
      },
      error: err => {
        this.error = err.message;
        this.loading = false;
      }
    });
  }
}
