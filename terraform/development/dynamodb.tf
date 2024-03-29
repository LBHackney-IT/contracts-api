resource "aws_dynamodb_table" "contractsapi_dynamodb_table" {
    name           = "Contracts"
    billing_mode   = "PROVISIONED"
    read_capacity  = 10
    write_capacity = 10
    hash_key       = "id"

    attribute {
        name = "id"
        type = "S"
    }

    attribute {
        name = "targetId"
        type = "S"
    }
	
	attribute {
		name = "targetContractNumber"
		type = "N"
	}

    global_secondary_index {
        name               = "ContractsByTargetId"
        hash_key           = "targetId"
		range_key          = "targetContractNumber"
        write_capacity     = 10
        read_capacity      = 10
        projection_type    = "ALL"
    }

    tags = merge(
        local.default_tags,
        { BackupPolicy = "Dev" }
    )

    point_in_time_recovery {
        enabled = true
    }
}
