terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }
}

provider "aws" {
  region = "eu-west-2"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
  default_tags = {
    Name              = "contracts-api-${var.environment_name}"
    Environment       = var.environment_name
    terraform-managed = true
    project_name      = var.project_name
  }
}

terraform {
  backend "s3" {
    bucket         = "housing-pre-production-terraform-state"
    encrypt        = true
    region         = "eu-west-2"
    key            = "services/contracts-api/state"
    dynamodb_table = "housing-pre-production-terraform-state-lock"
  }
}

resource "aws_sns_topic" "contracts" {
  name                        = "contracts.fifo"
  fifo_topic                  = true
  content_based_deduplication = true
  kms_master_key_id           = "alias/aws/sns"
}

resource "aws_ssm_parameter" "contracts_sns_arn" {
  name  = "/sns-topic/pre-production/contracts/arn"
  type  = "String"
  value = aws_sns_topic.contracts.arn
}
