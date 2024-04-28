# docker-bake.hcl
group "default" {
  targets = ["ImageProcessing", "SecurityBase","ServicesSchedulerLib","SharedTypes","TypeExtensions","SSOlogon","QR","FireflyBox"]
}

variable "NEXUS_API_KEY" {
  default = "121d0000-0f4c-314a-8674-ca3f2db5847a"
}
variable "BUILD_NUMBER" {
  default = "1.0.0"
}

variable "containerType" {
  default = "Dockerfile"
}

target "ImageProcessing" {
  context = "./src/ImageProcessing"
  dockerfile = "${containerType}"
  tags = ["eldan/image-processing:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  // platforms = ["linux/amd64", "linux/arm64"]
}

target "SecurityBase" {
  context = "./src/SecurityBase"
  dockerfile = "${containerType}"
  tags = ["eldan/security-base:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
}

target "ServicesSchedulerLib" {
  context = "./src/ServicesSchedulerLib"
  dockerfile = "${containerType}"
  tags = ["eldan/services-scheduler-lib:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }

}

target "SharedTypes" {
  context = "./src/SharedTypes"
  dockerfile = "${containerType}"
  tags = ["eldan/shared-types:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "TypeExtensions" {
  context = "./src/TypeExtensions"
  dockerfile = "${containerType}"
  tags = ["eldan/type-extensions:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
}

target "SSOlogon" {
  context = "./src/SSOlogon"
  dockerfile = "${containerType}"
  tags= ["eldan/sso-logon:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "QR" {
  context = "./src/QR"
  dockerfile = "${containerType}"
  tags= ["eldan/qr:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "FireflyBox" {
  context = "./src/FireflyBox"
  dockerfile = "${containerType}"
  tags= ["eldan/qr:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}