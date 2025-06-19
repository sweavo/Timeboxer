if (-not ([Security.Principal.WindowsPrincipal] `
        [Security.Principal.WindowsIdentity]::GetCurrent()
        ).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    write-error "Need to be elevated"
    exit 1
}

    
choco install microsoft-build-tools -y
choco install netfx-4.8-devpack -y
