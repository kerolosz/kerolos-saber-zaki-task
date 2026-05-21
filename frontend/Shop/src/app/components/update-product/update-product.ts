import { ChangeDetectorRef, Component, NgZone, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Productservice } from '../../services/productservice';
import { Product } from '../../models/Product';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-update-product',
  standalone: false,
  templateUrl: './update-product.html',
  styleUrl: './update-product.css',
})
export class UpdateProduct implements OnInit {
  product = signal<Product>({} as Product);

  onFileChangeEventClicked: boolean = false;
  form!: FormGroup;
  imagePreview: string | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private _Productservice: Productservice,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.initializeForm();

    this.getProductData();
  }

  getProductData() {
    this.route.params.subscribe((params) => {
      const id = params['id'];
      this._Productservice.getProductByID(id).subscribe((response) => {
        if (response.success) {
          this.product.set(response.data);
          this.form.patchValue({
            id: this.product().id,
            name: this.product().name,
            description: this.product().description,
            oldPrice: this.product().oldPrice,
            newPrice: this.product().newPrice,
          });
        } else {
          console.error('Failed to fetch product details');
        }
      });
    });
  }
  initializeForm() {
    this.form = this.fb.group({
      id: [''],
      name: ['', Validators.required],
      description: ['', Validators.required],
      oldPrice: [null, Validators.required],
      newPrice: [null, Validators.required],
      image: [null],
      updatedBy: [''],
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

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.ngZone.run(() => {
          this.imagePreview = reader.result as string;
          this.cdr.detectChanges();
        });
      };

      reader.readAsDataURL(file);
    } else {
      this.form.patchValue({
        image: null,
      });

      this.ngZone.run(() => {
        this.imagePreview = null;
        this.cdr.detectChanges();
      });

      this.form.get('image')?.updateValueAndValidity();
    }
  }

  submit() {
    if (this.form.invalid) return;

    const formData = new FormData();

    formData.append('id', this.form.value.id);
    formData.append('name', this.form.value.name);
    formData.append('description', this.form.value.description);
    formData.append('oldPrice', this.form.value.oldPrice!.toString());
    formData.append('newPrice', this.form.value.newPrice!.toString());
    formData.append('image', this.form.value.image);
    formData.append('updatedBy', 'admin'); // TODO: Replace with actual user

    this._Productservice.updateProduct(formData).subscribe({
      next: (res) => {
        this.form.reset();

        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
