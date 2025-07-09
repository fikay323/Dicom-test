import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { DicomApiService } from '../../services/dicom-api.service';

@Component({
  selector: 'app-viewer',
  imports: [FormsModule],
  templateUrl: './viewer.component.html',
  styleUrls: ['./viewer.component.scss']
})
export class ViewerComponent {
  selectedFile: File | null = null;
  previewUrl: SafeUrl | null = null;
  loading = false;
  dicomTag: string = '';
  tagResult: { tag: string, value: string } | null = null;
  error: string | null = null;
  uploadedFileName: string = '';

  constructor(private readonly dicomApi: DicomApiService, private readonly sanitizer: DomSanitizer) { }

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
        this.uploadedFileName = res.filename;
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

  checkTag(): void {
    if (!this.selectedFile || !this.uploadedFileName || !this.dicomTag) return;

    this.loading = true;
    this.tagResult = null;
    this.error = null;

    this.dicomApi.getDicomTag(this.uploadedFileName, this.dicomTag).subscribe({
      next: result => {
        this.tagResult = result;
        this.loading = false;
      },
      error: err => {
        this.error = err.message;
        this.loading = false;
      }
    });
  }

}
