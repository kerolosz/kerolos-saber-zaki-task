import { Component, OnInit } from '@angular/core';
import { Productservice } from '../../services/productservice';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-product',
  standalone: false,
  templateUrl: './create-product.html',
  styleUrl: './create-product.css',
})
export class CreateProduct implements OnInit {
  onFileChangeEventClicked: boolean = false;
  form!: FormGroup;
  constructor(
    private fb: FormBuilder,
    private productService: Productservice,
    private router: Router,
  ) {}
  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      oldPrice: [null, Validators.required],
      newPrice: [null, Validators.required],
      image: [null, Validators.required],
      createdBy: [''],
    });
  }

  onFileChange(event: any) {
    this.onFileChangeEventClicked = true;

    if (event.target.files.length > 0) {
      const file = event.target.files[0];

      this.form.patchValue({
        image: file,
      });

      this.form.get('image')?.updateValueAndValidity();
    } else {
      this.form.patchValue({
        image: null,
      });

      this.form.get('image')?.updateValueAndValidity();
    }
  }

  submit() {
    if (this.form.invalid) return;

    const formData = new FormData();

    formData.append('name', this.form.value.name!);
    formData.append('description', this.form.value.description!);
    formData.append('oldPrice', this.form.value.oldPrice!.toString());
    formData.append('newPrice', this.form.value.newPrice!.toString());
    formData.append('image', this.form.value.image);
    formData.append('createdBy', 'admin'); // TODO: Replace with actual user

    this.productService.createProduct(formData).subscribe({
      next: (res) => {
        console.log('Created:', res);
        this.form.reset();

        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
