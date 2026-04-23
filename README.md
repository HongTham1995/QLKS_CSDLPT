# 🗄️ Hệ thống Quản lý Khách sạn – Cơ sở dữ liệu phân tán

## 📌 Tổng quan dự án

Hệ thống quản lý khách sạn được xây dựng theo mô hình cơ sở dữ liệu phân tán, hỗ trợ quản lý phòng, khách hàng, nhân viên, dịch vụ, đặt phòng và hóa đơn.

Hệ thống được thiết kế nhằm đảm bảo tính nhất quán dữ liệu, hiệu suất và khả năng mở rộng giữa nhiều chi nhánh.

---

## 🎯 Mục tiêu hệ thống

- Quản lý phòng, khách hàng, nhân viên và dịch vụ  
- Hỗ trợ đặt phòng theo thời gian thực  
- Quản lý hóa đơn và thanh toán  
- Thống kê doanh thu theo nhiều tiêu chí  
- Thiết kế và triển khai cơ sở dữ liệu phân tán  

---

## 👥 Phân công & đóng góp

- 🗄️ Database & System Design (ERD, RDM, SQL, phân tán dữ liệu): Nguyễn Thị Hồng Thắm
- 💻 UI (C# WinForms): Phan Phước Vinh  

---

## 🧠 Phần tôi phụ trách

Tôi chịu trách nhiệm thiết kế toàn bộ hệ thống cơ sở dữ liệu và logic phân tán dữ liệu.

---

## 🧩 Thiết kế hệ thống

### 📌 ERD (Entity Relationship Diagram)

<img width="974" height="528" alt="image" src="https://github.com/user-attachments/assets/cfdd2a78-1c93-4e9b-8830-d8cd3a54e4bb" />

### 📌 Lược đồ cơ sở dữ liệu
<img width="945" height="548" alt="image" src="https://github.com/user-attachments/assets/58c1d0d6-da31-46f8-a79e-e2ae89114fa3" />


### 📌 Kiến trúc cơ sở dữ liệu phân tán

          CLIENT / APPLICATION
                    |
        -------------------------
        |           |           |
     CN1 (HN)    CN2 (Huế)   CN3 (HCM)
        |           |           |
     SQL DB      SQL DB      SQL DB

## 🌐 Kiến trúc cơ sở dữ liệu phân tán
  
  Hệ thống được triển khai theo mô hình **Replication trong SQL Server**, gồm 3 chi nhánh:
  
  - CN1 (Hà Nội)
  - CN2 (Huế)
  - CN3 (TP Hồ Chí Minh)
  
  Dữ liệu được phân tán và đồng bộ giữa các server nhằm đảm bảo tính nhất quán.
  
  ### 📌 Cấu hình Replication trong SQL Server
 <img width="628" height="607" alt="image" src="https://github.com/user-attachments/assets/14b2e8a5-4aa4-4da5-b582-16ff6c2b0c52" />

  ---

## 🏨 Một số giao diện hệ thống

### 📌 Màn hình chính

- CN1: Hà Nội (HN)
  
  <img width="945" height="507" alt="image" src="https://github.com/user-attachments/assets/60d1feea-4af8-4c8f-b79c-f3b9e1182fc0" />
- CN3: TP.HCM
  
  <img width="945" height="532" alt="image" src="https://github.com/user-attachments/assets/c56d0e28-3275-49dc-a20a-5c9b8b2b8ac0" />


---

### 📌 Thống kê hệ thống
- CN1: Hà Nội (HN)

  <img width="945" height="532" alt="image" src="https://github.com/user-attachments/assets/b59e2602-cf03-4e71-897e-31507c57f3f9" />
- CN2: Huế
  
  <img width="945" height="532" alt="image" src="https://github.com/user-attachments/assets/25c61b81-d772-4a6f-bdca-e7d84d89c4b3" />
- CN3: TP.HCM
  
  <img width="945" height="532" alt="image" src="https://github.com/user-attachments/assets/1b7c18c6-1169-41b8-8f6f-fd36668ee967" />

---

## ⚙️ Công nghệ sử dụng

- SQL Server  
- Distributed Database System  
- Database Replication  
- C# WinForms (UI)  
- ERD / RDM Design  

---

## 📊 Kết quả đạt được

- Thiết kế thành công hệ thống cơ sở dữ liệu phân tán  
- Đảm bảo đồng bộ dữ liệu giữa nhiều chi nhánh  
- Xây dựng truy vấn SQL phục vụ toàn bộ nghiệp vụ  
- Hỗ trợ đầy đủ quản lý khách sạn thực tế  

---

## 💡 Kỹ năng đạt được

- Thiết kế cơ sở dữ liệu quan hệ  
- Mô hình hóa hệ thống (ERD, RDM)  
- Hệ thống phân tán dữ liệu  
- SQL nâng cao  
- System design tư duy backend  

---

## 🚀 Hướng phát triển

- Tối ưu hệ thống phân tán quy mô lớn  
- Chuyển sang microservices  
- Tích hợp API backend  
- Deploy cloud database  

---
