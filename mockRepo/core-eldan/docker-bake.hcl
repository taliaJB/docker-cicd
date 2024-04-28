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

variable "Dockerfile_Name" {
  default = "DockerfileWindows"
}


target "test" {
  context = ".\\src\\test"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/image-processing:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  // platforms = ["linux/amd64", "linux/arm64"]
}

target "ImageProcessing" {
  context = ".\\src\\ImageProcessing"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/image-processing:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  // platforms = ["linux/amd64", "linux/arm64"]
}

target "SecurityBase" {
  context = ".\\src\\SecurityBase"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/security-base:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
}

target "ServicesSchedulerLib" {
  context = ".\\src\\ServicesSchedulerLib"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/services-scheduler-lib:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }

}

target "SharedTypes" {
  context = ".\\src\\SharedTypes"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/shared-types:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "TypeExtensions" {
  context = ".\\src\\TypeExtensions"
  dockerfile = "${Dockerfile_Name}"
  tags = ["eldan/type-extensions:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
}

target "SSOlogon" {
  context = ".\\src\\SSOlogon"
  dockerfile = "${Dockerfile_Name}"
  tags= ["eldan/sso-logon:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "QR" {
  context = ".\\src\\QR"
  dockerfile = "${Dockerfile_Name}"
  tags= ["eldan/qr:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}

target "FireflyBox" {
  context = ".\\src\\FireflyBox"
  dockerfile = "${Dockerfile_Name}"
  tags= ["eldan/qr:latest"]
  args = {
    NEXUS_API_KEY = "${NEXUS_API_KEY}",
    BUILD_NUMBER = "${BUILD_NUMBER}"
  }
  
}