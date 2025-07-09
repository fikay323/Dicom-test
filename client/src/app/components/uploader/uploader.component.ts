import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-uploader',
  imports: [CommonModule],
  templateUrl: './uploader.component.html',
  styleUrl: './uploader.component.scss'
})
export class UploaderComponent {
  @Input() disabled = false;
  @Output() fileSelected = new EventEmitter<File>();

  isDragOver = false;
  file: File | null = null;

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    if (!this.disabled) {
      this.isDragOver = true;
    }
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
    if (!this.disabled) {
      const files = event.dataTransfer?.files;
      if (files && files.length > 0) {
        this.handleFile(files[0]);
      }
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFile(input.files[0]);
    }
  }

  handleFile(selectedFile: File) {
    this.file = selectedFile;
    this.fileSelected.emit(this.file);
  }

  removeFile() {
    this.file = null;
    // Note: We don't emit an event on removal, as the parent component handles the reset.
  }

  formatBytes(bytes: number, decimals = 2): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
