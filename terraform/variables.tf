# Define variables for resource names and location
variable "resource_group_name" {
  default = "QuizGameResourceGroup"
}

variable "location" {
  default = "East US"
}

variable "vm_name" {
  default = "QuizGameVM"
}

variable "admin_username" {
  default = "azureuser"
}

variable "admin_password" {
  description = "Password for the VM admin user"
  type        = string
  sensitive   = true
}